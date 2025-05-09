using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using FieldBookingAPI.Data;
using FieldBookingAPI.Helpers;
using FieldBookingAPI.Models;

namespace FieldBookingAPI.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtTokenGenerator _jwt;

        public AuthService(AppDbContext context, JwtTokenGenerator jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        public async Task<string?> RegisterAsync(string name, string identifier, string password)
        {           
            var exists = await _context.Users.AnyAsync(u => u.Identifier == identifier);
            if (exists) return null;

            var user = new User
            {
                Name = name,
                Identifier = identifier,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "member"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return _jwt.GenerateToken(user);
        }


        public async Task<string?> LoginAsync(string identifier, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Identifier == identifier);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;

            return _jwt.GenerateToken(user);
        }
    }
}
