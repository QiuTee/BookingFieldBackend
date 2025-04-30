namespace FieldBookingAPI.DTOs
{
    public class RegisterDto
    {
        public string Name { get; set; } = null!;
        public string Identifier { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string RetypePassword { get; set; } = null! ; 
    }

    public class LoginDto
    {
        public string Identifier { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
