using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FieldBookingAPI.Data;
using FieldBookingAPI.DTOs;


namespace FieldBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoucherController : ControllerBase
    {
        public readonly AppDbContext _context;

        public VoucherController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyVoucher([FromBody] ApplyVoucherDto dto) {
            var nowUtc = DateTime.UtcNow;
            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Code == dto.Code && v.IsActive && v.ExpireDate > nowUtc);
            
            if (voucher == null)
            {
                return BadRequest(new { message = "Không tìm thấy voucher hoặc voucher hết hạn" , status = "not_found" });
            }
            Console.WriteLine("\nVoucher " + voucher.DiscountValue);
            if (dto.TotalPrice < voucher.MinAmount)
            {
                return BadRequest(new { message = "Bạn chưa đủ điều kiện để sử dụng voucher này" , status = "min_amount_not_met"});
            }
            Console.WriteLine("\nTotal Price " + dto.TotalPrice * (voucher.DiscountValue / 100));
            int discountAmount = voucher.DiscountType == "percentage"
                ? dto.TotalPrice * voucher.DiscountValue / 100
                : voucher.DiscountValue;

            return Ok(new { discountAmount, voucherId = voucher.Id ,status = "success"});
        
        }
    }

    
}