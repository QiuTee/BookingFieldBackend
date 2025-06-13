namespace FieldBookingAPI.DTOs
{
    public class PaymentConfirmationDto
    {
        public string PaymentImageUrl { get; set; } = null!;
        public string? StudentCardImageUrl { get; set; }
        public string? VoucherCode { get; set; }
        public int? DiscountAmount { get; set; }
        public int? VoucherId { get; set; }
    }
}