namespace FieldBookingAPI.DTOs
{
    public class PaymentConfirmationDto
    {
        public string PaymentImageUrl { get; set; } = null!;
        public string? StudentCardImageUrl { get; set; }
    }
}