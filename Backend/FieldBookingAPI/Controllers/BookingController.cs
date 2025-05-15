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
                FieldId = dto.FieldId,
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

        [AllowAnonymous]
        [HttpPut("{id}/confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(int id, [FromBody] PaymentConfirmationDto  dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();
            
            booking.Status = "paid";
            booking.PaymentImageUrl = dto.PaymentImageUrl;
            booking.StudentCardImageUrl = dto.StudentCardImageUrl;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Xác nhận thanh toán thành công" });
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
                .Where(b => b.FieldName == fieldName && b.Date == safeDate && ( b.Status == "confirmed" || b.Status == "paid"))
                .SelectMany(b => b.Slots)
                .Select(s => new { s.SubField, s.Time, Status = s.Booking.Status })
                .ToListAsync();

            return Ok(slots);
        }

        [HttpGet("public/{id}")]
        public async Task<IActionResult> GetPublicBookingById(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Slots)
                .FirstOrDefaultAsync(b => b.Id == id && ( b.Status == "confirmed" || b.Status == "pending"));

            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        [HttpGet("owner-bookings/{slug}")]
        [Authorize(Roles = "admin, owner")]
        public async Task<IActionResult> GetBookingForOnwer(string slug)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var field = await _context.Fields
                .FirstOrDefaultAsync(f => f.Slug == slug && f.OwnerId == userId);
            
            if (field == null){
                return Forbid();
            }

            if (field == null){
                return Forbid();
            }
            var bookings = await _context.Bookings
                .Include(b => b.Slots)
                .Where(b => b.FieldId == field.Id)
                .OrderByDescending(b => b.Date)
                .ToListAsync();

            return Ok(bookings);
        }

        [HttpPut("{id}/update-status")]
        [Authorize(Roles = "admin, owner")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody]  StatusUpdateDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            booking.Status = dto.Status.ToLower();
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật trạng thái đơn thành công" });
        }

        [Authorize(Roles = "admin, owner")]
        [HttpPut("{id}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var booking = await _context.Bookings
                .Include(b => b.Field)
                .FirstOrDefaultAsync(b => b.Id == id && b.Field != null && b.Field.OwnerId == userId);

            if (booking == null)
                return NotFound();

            booking.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã đánh dấu đã đọc" });
        }

    }
}
