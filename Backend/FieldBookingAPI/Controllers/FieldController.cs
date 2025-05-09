using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using FieldBookingAPI.DTOs;
using FieldBookingAPI.Models;
using FieldBookingAPI.Data;
using FieldBookingAPI.Utils;


namespace FieldBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FieldController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FieldController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateField([FromBody] CreateFieldDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var baseSlug = SlugHelper.GenerateSlug(dto.Name);
            var slug = baseSlug;
            int counter = 1;

            while (await _context.Fields.AnyAsync(f => f.Slug == slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }
            var field = new Field{
                Name = dto.Name,
                Location = dto.Location,
                Phone = dto.Phone, 
                Type = dto.Type,
                Price = dto.Price,
                Opentime = dto.Opentime,
                Closetime = dto.Closetime,
                Is24h = dto.Is24h,
                HeroImage = dto.HeroImage,
                Logo = dto.Logo,
                Slug = slug,
                OwnerId = userId,
            };
            _context.Fields.Add(field);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Sân đã được tạo thành công", fieldId = field.Id });
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetFieldBySlug(string slug)
        {
            var field = await _context.Fields
                .Include(f => f.Images)
                .Include(f => f.Services)
                .Include(f => f.Reviews)
                .Include(f => f.SubFields)
                .FirstOrDefaultAsync(f => f.Slug == slug);

            if (field == null)
            {
                return NotFound(new { message = "Không tìm thấy sân" });
            }

            return Ok(field);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateField(int id, [FromBody] Field updatedField)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var field = await _context.Fields.FindAsync(id);
            var userRoke = User.FindFirst(ClaimTypes.Role)?.Value;

            if(field == null )
            {
                return NotFound();
            }

            if (userRoke != "admin" && field.OwnerId != userId)
            {
                return Forbid("Bạn không có quyền sửa sân này");
            }


            field.Name = updatedField.Name;
            field.Location = updatedField.Location;
            field.Phone = updatedField.Phone;
            field.Price = updatedField.Price;
            field.Type = updatedField.Type;
            field.Opentime = updatedField.Opentime;
            field.Closetime = updatedField.Closetime;
            field.Is24h = updatedField.Is24h;
            field.Logo = updatedField.Logo;
            field.HeroImage = updatedField.HeroImage;            

            await _context.SaveChangesAsync();
            return Ok(field);
        }

        [HttpGet("my-fields")]
        [Authorize]
        public async Task<IActionResult> GetMyFields()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var fields = await _context.Fields
                .Where(f => f.OwnerId == userId)
                .Include(f => f.Images)
                .Include(f => f.Services)
                .Include(f => f.Reviews)
                .Include(f => f.SubFields)
                .ToListAsync();

            return Ok(fields);
        }
        
    }


}