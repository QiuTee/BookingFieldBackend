namespace FieldBookingAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Identifier { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "member"; 
        public List<Field> Fields { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
    
    }
}
