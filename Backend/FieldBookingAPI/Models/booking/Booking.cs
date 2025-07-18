using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace FieldBookingAPI.Models
{
    public class Booking
{
    public int Id { get; set; }
    public string BookingCode { get; set; } = null!;
    public string FieldName { get; set; } = null!;
    public DateTime Date { get; set; }
    public string UserName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Notes { get; set; }
    public string Status { get; set; } = "unpaid"; 
    public string? PaymentImageUrl { get; set; }
    public string? StudentCardImageUrl { get; set; }
    public bool IsRead { get; set; } = false;
    public int TotalPrice { get; set; } = 0;
    public string? ProcessStatus { get; set; }
    public int? UserId { get; set; } 
    public string? VoucherCode { set; get; }
    public int? DiscountAmount { set; get; }
    public int? VoucherId { get; set; }
    public Voucher? Voucher { get; set; }
    public User? User { get; set; }
    public int FieldId { get; set; }
    public Field? Field { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<BookingSlot> Slots { get; set; } = new(); 
}

}