namespace FieldBookingAPI.DTOs
{
    public class CreateFieldDto
    {
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int Price { get; set; }
        public string? Opentime { get; set; } 
        public string? Closetime { get; set; }
        public bool Is24h { get; set; } 
        public string? HeroImage { get; set; }
        public string? Logo { get; set; }
    }
}