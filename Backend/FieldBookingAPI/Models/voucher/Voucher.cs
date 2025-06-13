
namespace FieldBookingAPI.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string DiscountType { get; set; } = "fixed";
        public int DiscountValue { get; set; }
        public int MinAmount { get; set; } = 0;
        public DateTime ExpireDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
