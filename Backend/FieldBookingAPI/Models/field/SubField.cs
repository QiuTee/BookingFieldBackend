namespace FieldBookingAPI.Models
{
    public class SubField
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = "active";
        public int FieldId { get; set; }
        public Field Field { get; set; } = null!;
        
    }
}