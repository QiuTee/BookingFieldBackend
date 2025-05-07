namespace FieldBookingAPI.Models
{
    public class Field
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? HeroImage { get; set; }
        public string? Logo { get; set; }
        public string Location { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Opentime { get; set; } 
        public string? Closetime { get; set; } 
        public string Type { get; set; } = null!;
        public int Price { get; set; } = 0;
        public bool Is24h { get; set; } 
        public int OwnerId { get; set; }
        public User Owner { get; set; } = null!;

        public List<FieldImage> Images { get; set; } = new();
        public List<FieldService> Services { get; set; } = new();
        public List<FieldReview>? Reviews { get; set; } = new();
        public List<SubField>? SubFields { get; set; } = new();
    }
}
