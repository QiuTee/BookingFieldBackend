using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using DotNetEnv;
using FieldBookingAPI.Services;
using FieldBookingAPI.Helpers;
using FieldBookingAPI.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.FileProviders;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application host");

    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        WebRootPath = "dist"
    });

    var config = builder.Configuration;
    var env = builder.Environment;

    builder.WebHost.UseWebRoot("dist");
    builder.Host.UseSerilog();

    Env.Load();

    var isEfCommand = AppDomain.CurrentDomain.FriendlyName.Contains("ef");

    string? jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
    if (!isEfCommand && string.IsNullOrWhiteSpace(jwtKey))
    {
        Log.Fatal("JWT_KEY not found in environment variables. Application is shutting down.");
        throw new InvalidOperationException("JWT_KEY not found in environment variables.");
    }

    if (!string.IsNullOrWhiteSpace(jwtKey))
    {
        builder.Configuration["Jwt:Key"] = jwtKey;
    }

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var connectionString = config.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(connectionString, sqlOptions => sqlOptions.CommandTimeout(30));
    });

    builder.Services.AddScoped<AuthService>();
    builder.Services.AddScoped<JwtTokenGenerator>();
    builder.Services.AddHostedService<BookingCleanupService>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
    });

    var keyDirectory = @"C:\data\keys\fieldbooking-keys";
    if (!Directory.Exists(keyDirectory))
    {
        Directory.CreateDirectory(keyDirectory);
    }

    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(keyDirectory))
        .SetApplicationName("FieldBookingAPI");

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    if (!string.IsNullOrEmpty(token)) context.Token = token;
                    return Task.CompletedTask;
                }
            };
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? "fallback_key"))
            };
        });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    if (env.IsDevelopment() || env.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();
    app.UseRouting();
    app.UseCors("AllowAll");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapFallbackToFile("index.html");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
