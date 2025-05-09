using Microsoft.AspNetCore.Mvc;
using FieldBookingAPI.DTOs;
using FieldBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FieldBookingAPI.Data;
namespace FieldBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly AppDbContext _context;

        public AuthController(AuthService authService , AppDbContext context)
        {
            _authService = authService;
            _context = context;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (dto.Password != dto.RetypePassword)
            {
                return BadRequest("Mật khẩu nhập lại không khớp");
            }

            var token = await _authService.RegisterAsync(dto.Name, dto.Identifier, dto.Password);
            if (token == null)
                return BadRequest(new { message = "Tài khoản đã tồn tại" });

                
            return Ok(new
            {
                status = "200",
                message = "Đăng ký thành công",
                token = token
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto.Identifier, dto.Password);
            if (token == null)
                return Unauthorized("Sai thông tin đăng nhập");

            return Ok(new
            {
                message = "Đăng nhập thành công",
                token = token
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Đăng xuất thành công" });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var name = User.Identity?.Name;

            return Ok(new
            {
                userId,
                name
            });
        }
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Controller đang hoạt động!");
        }

        [HttpGet("check-register")]
        public IActionResult CheckRegister()
        {
            var method = typeof(AuthController).GetMethods()
                .FirstOrDefault(m => m.Name == "Register");

            return Ok(method?.Name ?? "Register not found");
        }

        [Authorize(Roles = "admin")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
           var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                return NotFound(new { message = "Người dùng không tồn tại" });
            }

            user.Role = dto.Role;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Phân quyền thành công" });
        }

    }
}
