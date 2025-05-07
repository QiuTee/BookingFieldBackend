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
        [Authorize]
        public async Task<IActionResult> CreateField([FromBody] Field field)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            field.OwnerId = userId;

            var baseSlug = SlugHelper.GenerateSlug(field.Name);
            var slug = baseSlug;
            int counter = 1;

            while (await _context.Fields.AnyAsync(f => f.Slug == slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }
            field.Slug = slug;
            _context.Fields.Add(field);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Sân đã được tạo thành công", fieldId = field.Id });
        }
    }


}