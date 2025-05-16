using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FieldBookingAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace FieldBookingAPI.Services
{
    public class BookingCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _intervaal = TimeSpan.FromMinutes(5);
        public BookingCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var now = DateTime.UtcNow;
                    var expiredTime = now.AddMinutes(-30);
                    var expiredBookings = await context.Bookings
                        .Where(b => b.Status == "unpaid" && b.CreatedAt < expiredTime)
                        .ToListAsync();
                    
                    if (expiredBookings.Any()){
                        context.Bookings.RemoveRange(expiredBookings);
                        await context.SaveChangesAsync(); 
                        Console.WriteLine($"[AutoCancel] Huỷ {expiredBookings.Count} đơn đặt lúc {DateTime.Now}");
                    }
                }

                await Task.Delay(_intervaal, stoppingToken);
            }   
        }       
    }
}