using System.Text.Json.Serialization;

namespace FieldBookingAPI.Models
{
    public class BookingSlot
    {
        public int Id { get; set; }
        public string SubField { get; set; } = null!;
        public string Time { get; set; } = null!;

        public int BookingId { get; set; }

        [JsonIgnore]
        public Booking Booking { get; set; } = null!;
    }
}
