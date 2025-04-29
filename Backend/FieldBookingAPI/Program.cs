using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using DotNetEnv;
using FieldBookingAPI.Services;
using FieldBookingAPI.Helpers;
using FieldBookingAPI.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var env = builder.Environment;

// Load environment variables (e.g., JWT_KEY)
DotNetEnv.Env.Load();
builder.Configuration["Jwt:Key"] = Environment.GetEnvironmentVariable("JWT_KEY");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database connection
var connectionString = config.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString, sqlOptions => sqlOptions.CommandTimeout(30));
});

// Add your services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtTokenGenerator>();

// Configure CORS for frontend (localhost & future domain)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173", 
                "https://your-frontend.vercel.app", 
                "https://yourcustomdomain.com"
            )
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured."))
            )
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// 👉 Bật Swagger trong cả Development và Production
if (env.IsDevelopment() || env.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middlewares
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// Map Controller routes
app.MapControllers();

app.Run();
