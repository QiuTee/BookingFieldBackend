namespace FieldBookingAPI.DTOs
{
    public class BookingDto
    {
        public string FieldName { get; set; } = null!;
        public DateTime Date { get; set; }

        public List<BookingSlotDto> Slots { get; set; } = new();
        public string UserName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Notes { get; set; }
    }
}
