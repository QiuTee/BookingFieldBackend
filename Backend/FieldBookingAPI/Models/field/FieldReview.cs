namespace FieldBookingAPI.Models
{
    public class FieldReview
    {
        public int Id { get; set; }
        public int Rating   { get; set; } 
        public string Comment { get; set; } = null!;
        public int FieldId { get; set; }
        public Field Field { get; set; } = null!;
    }
}