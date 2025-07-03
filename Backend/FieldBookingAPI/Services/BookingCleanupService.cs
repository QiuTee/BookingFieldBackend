using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FieldBookingAPI.Data;
using Microsoft.Extensions.Logging;

namespace FieldBookingAPI.Services
{
    public class BookingCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BookingCleanupService> _logger;

        public BookingCleanupService(IServiceScopeFactory scopeFactory, ILogger<BookingCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var expiredTime = DateTime.UtcNow.AddMinutes(-30);
                        var expiredBookings = await context.Bookings
                            .Where(b => b.Status == "unpaid" && b.CreatedAt < expiredTime)
                            .ToListAsync(stoppingToken);
                        
                        if (expiredBookings.Any())
                        {
                            context.Bookings.RemoveRange(expiredBookings);
                            await context.SaveChangesAsync(stoppingToken); 
                            _logger.LogInformation($"[AutoCancel] Cancelled {expiredBookings.Count} expired bookings.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in BookingCleanupService execution.");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}