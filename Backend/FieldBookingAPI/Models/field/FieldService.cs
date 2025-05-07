namespace FieldBookingAPI.Models
{
    public class FieldService
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int FieldId { get; set; } 
        public Field Field { get; set; } = null!;
    }
}