using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using FieldBookingAPI.DTOs;
using FieldBookingAPI.Models;
using FieldBookingAPI.Data;

namespace FieldBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var booking = new Booking
            {
                FieldName = dto.FieldName,
                Date = dto.Date,
                UserName = dto.UserName,
                Phone = dto.Phone,
                Notes = dto.Notes,
                Status = "pending",
                UserId = int.Parse(userIdClaim),
                Slots = dto.Slots.Select(s => new BookingSlot
                {
                    SubField = s.SubField,
                    Time = s.Time
                }).ToList()
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đặt sân thành công", bookingId = booking.Id });
        }

        [HttpGet("my-bookings")]
        [Authorize]
        public async Task<IActionResult> GetUserBookings()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var bookings = await _context.Bookings
                .Include(b => b.Slots)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Date)
                .ToListAsync();

            return Ok(bookings);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var booking = await _context.Bookings
                .Include(b => b.Slots)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        [HttpDelete("auto-cancel")]
        public async Task<IActionResult> AutoCancelBookings()
        {
            var now = DateTime.UtcNow;
            var expiredTime = now.AddMinutes(-30);

            var expiredBookings = await _context.Bookings
                .Where(b => b.Status == "pending" && b.CreatedAt <= expiredTime)
                .ToListAsync();

            if (!expiredBookings.Any())
            {
                return Ok(new { message = "Không có đơn nào cần huỷ." });
            }

            _context.Bookings.RemoveRange(expiredBookings);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã huỷ {expiredBookings.Count} đơn quá hạn thanh toán." });
        }

        [HttpGet("booked-slots")]
        public async Task<IActionResult> GetBookedSlots([FromQuery] string fieldName, [FromQuery] DateTime date)
        {
            var slots = await _context.Bookings
                .Where(b => b.FieldName == fieldName && b.Date == date && b.Status == "confirmed")
                .SelectMany(b => b.Slots)
                .Select(s => new { s.SubField, s.Time })
                .ToListAsync();

            return Ok(slots);
        }

    }
}
