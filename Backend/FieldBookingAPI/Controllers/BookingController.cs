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
        public async Task<IActionResult> CreateBooking([FromBody] BookingDto dto)
        {
            int? userId = null;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!string.IsNullOrEmpty(userIdClaim)){
                userId = int.Parse(userIdClaim);
            }

            var safeDate = DateTime.SpecifyKind(dto.Date.Date, DateTimeKind.Utc);

            var booking = new Booking
            {
                FieldName = dto.FieldName,
                Date = safeDate,
                UserName = dto.UserName,
                Phone = dto.Phone,
                Notes = dto.Notes,
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                UserId = userId ,
                Slots = dto.Slots.Select(s => new BookingSlot
                {
                    SubField = s.SubField,
                    Time = s.Time
                }).ToList()
            };

            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Đặt sân thành công", bookingId = booking.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lưu booking: " + ex.ToString());
                return StatusCode(500, "Lỗi khi lưu booking");
            }
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
            var nowUtc = DateTime.UtcNow;
            var expiredTime = nowUtc.AddMinutes(-30);

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
            var safeDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

            var slots = await _context.Bookings
                .Where(b => b.FieldName == fieldName && b.Date == safeDate && b.Status == "confirmed")
                .SelectMany(b => b.Slots)
                .Select(s => new { s.SubField, s.Time })
                .ToListAsync();

            return Ok(slots);
        }

        [HttpGet("public/{id}")]
        public async Task<IActionResult> GetPublicBookingById(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Slots)
                .FirstOrDefaultAsync(b => b.Id == id && b.Status == "confirmed");

            if (booking == null)
                return NotFound();

            return Ok(booking);
        }


    }
}
