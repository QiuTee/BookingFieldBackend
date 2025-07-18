using FieldBookingAPI.Models;

namespace FieldBookingAPI.DTOs
{
    public class CreateFieldDto
    {
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int Price { get; set; }
        public string? FaceBook{ get; set; }
        public string? Opentime { get; set; }
        public string? Closetime { get; set; }
        public string? Status { get; set; }
        public bool Is24h { get; set; }
        public string? HeroImage { get; set; }
        public string? Logo { get; set; }
        public int? OwnerId { get; set; }
        public List<string>? ImageUrls { get; set; }
        public List<ServiceDto>? Services { get; set; }
        public List<SubFieldDto>? SubFields { get; set; }
    }
}