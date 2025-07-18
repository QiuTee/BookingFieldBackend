using FieldBookingAPI.Models;

namespace FieldBookingAPI.DTOs
{
    public class FieldDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? HeroImage { get; set; }
        public string? Logo { get; set; }
        public string Location { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int Price { get; set; }
        public bool Is24h { get; set; }
        public string? Opentime { get; set; }
        public string? FaceBook{ get; set; }
        public string? Closetime { get; set; }
        public string? Status { get; set; }
        public int OwnerId { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<ServiceDto> Services { get; set; } = new();
        public List<SubFieldDto> SubFields { get; set; } = new();
        public List<string> Reviews { get; set; } = new();
    }
}
