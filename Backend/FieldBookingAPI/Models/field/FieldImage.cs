namespace FieldBookingAPI.Models
{
    public class FieldImage
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public int FieldId { get; set; } 
        public Field Field { get; set; } = null!;
        
    }
}