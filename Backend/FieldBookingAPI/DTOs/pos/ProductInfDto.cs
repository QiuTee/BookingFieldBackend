using FieldBookingAPI.Models;

namespace FieldBookingAPI.DTOs
{
    public class ProductInfDto
    {
        public string Name { get; set; } = null!;
        public int Price { get; set; } = 0;
        public string Category { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool Available { get; set; } = true; 
   }
}