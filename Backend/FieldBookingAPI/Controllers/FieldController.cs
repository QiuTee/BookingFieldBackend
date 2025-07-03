using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using FieldBookingAPI.DTOs;
using FieldBookingAPI.Models;
using FieldBookingAPI.Data;
using FieldBookingAPI.Utils;
using System.Reflection;


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
        [Authorize(Roles = "admin, owner")]
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
            var field = new Field
            {
                Name = dto.Name,
                Location = dto.Location,
                Phone = dto.Phone,
                Type = dto.Type,
                Price = dto.Price,
                Opentime = dto.Opentime,
                Closetime = dto.Closetime,
                FaceBook = dto.FaceBook,
                Is24h = dto.Is24h,
                HeroImage = dto.HeroImage,
                Status = dto.Status ?? "active",
                Logo = dto.Logo,
                Slug = slug,
                OwnerId = dto.OwnerId ?? userId,
                CreatedByAdminId = userId,
            };
            _context.Fields.Add(field);
            await _context.SaveChangesAsync();

            if (dto.ImageUrls != null && dto.ImageUrls.Count > 0)
            {
                var image = dto.ImageUrls.Select(url => new FieldImage
                {
                    FieldId = field.Id,
                    Url = url
                });
                _context.FieldImages.AddRange(image);
            }

            if (dto.Services != null && dto.Services.Count > 0)
            {
                var service = dto.Services.Select(s => new FieldService
                {
                    FieldId = field.Id,
                    Name = s.Name,
                    Price = s.Price
                });
                _context.FieldServices.AddRange(service);
            }

            if (dto.SubFields != null && dto.SubFields.Count > 0)
            {
                var subField = dto.SubFields.Select(s => new SubField
                {
                    FieldId = field.Id,
                    Name = s.Name,
                    Status = s.Status ?? "status"
                });
                _context.SubFields.AddRange(subField);
            }
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
                .AsSplitQuery()
                .FirstOrDefaultAsync(f => f.Slug == slug);

            if (field == null)
            {
                return NotFound(new { message = "Không tìm thấy sân" });
            }
            var dto = new FieldDto
            {
                Id = field.Id,
                Name = field.Name,
                Slug = field.Slug,
                HeroImage = field.HeroImage,
                Logo = field.Logo,
                Location = field.Location,
                Phone = field.Phone,
                Type = field.Type,
                Price = field.Price,
                Is24h = field.Is24h,
                Opentime = field.Opentime,
                Closetime = field.Closetime,
                OwnerId = field.OwnerId,
                FaceBook = field.FaceBook,
                ImageUrls = field.Images?.Select(img => img.Url).ToList() ?? new(),
                SubFields = field.SubFields?.Select(s => new SubFieldDto
                {
                    Name = s.Name,
                    Status = s.Status
                }
                ).ToList() ?? new(),
                Reviews = field.Reviews?.Select(r => $"⭐ {r.Rating}: {r.Comment}").ToList() ?? new(),
                Services = field.Services?.Select(s => new ServiceDto
                {
                    Name = s.Name,
                    Price = s.Price
                }).ToList() ?? new(),
            };

            return Ok(dto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateField(int id, [FromBody] Field updatedField)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var field = await _context.Fields.FindAsync(id);
            var userRoke = User.FindFirst(ClaimTypes.Role)?.Value;

            if (field == null)
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
        [Authorize(Roles = "owner,admin")]
        public async Task<IActionResult> GetMyFields()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var fields = await _context.Fields
                .Where(f => f.OwnerId == userId)
                .Include(f => f.Images)
                .Include(f => f.Services)
                .Include(f => f.Reviews)
                .Include(f => f.SubFields)
                .AsSplitQuery()
                .ToListAsync();

            var fieldDtos = fields.Select(field => new FieldDto
            {
                Id = field.Id,
                Name = field.Name,
                Slug = field.Slug,
                HeroImage = field.HeroImage,
                Logo = field.Logo,
                Location = field.Location,
                Phone = field.Phone,
                Type = field.Type,
                Price = field.Price,
                Is24h = field.Is24h,
                Opentime = field.Opentime,
                Closetime = field.Closetime,
                OwnerId = field.OwnerId,
                Status = field.Status,
                ImageUrls = field.Images?.Select(img => img.Url).ToList() ?? new(),
                Services = field.Services?.Select(s => new ServiceDto
                {
                    Name = s.Name,
                    Price = s.Price
                }).ToList() ?? new(),
                SubFields = field.SubFields?.Select(s => new SubFieldDto
                {
                    Name = s.Name, 
                    Status = s.Status
                }
                ).ToList() ?? new(),
                Reviews = field.Reviews?.Select(r => $"⭐ {r.Rating}: {r.Comment}").ToList() ?? new()
            }).ToList();

            return Ok(fieldDtos);
        }

        [HttpGet]
        [AllowAnonymous] 
        public async Task<IActionResult> GetAllFields()
        {
            var fields = await _context.Fields
                .Include(f => f.Images)
                .Include(f => f.Services)
                .Include(f => f.Reviews)
                .Include(f => f.SubFields)
                .AsSplitQuery()
                .ToListAsync();

            var fieldDtos = fields.Select(field => new FieldDto
            {
                Id = field.Id,
                Name = field.Name,
                Slug = field.Slug,
                HeroImage = field.HeroImage,
                Logo = field.Logo,
                Location = field.Location,
                Phone = field.Phone,
                Type = field.Type,
                Price = field.Price,
                Is24h = field.Is24h,
                Opentime = field.Opentime,
                Closetime = field.Closetime,
                OwnerId = field.OwnerId,
                Status = field.Status,
                ImageUrls = field.Images?.Select(img => img.Url).ToList() ?? new(),
                Services = field.Services?.Select(s => new ServiceDto
                {
                    Name = s.Name,
                    Price = s.Price
                }).ToList() ?? new(),
                SubFields = field.SubFields?.Select(s => new SubFieldDto
                {
                    Name = s.Name, 
                    Status = s.Status
                }).ToList() ?? new(),
                Reviews = field.Reviews?.Select(r => $"⭐ {r.Rating}: {r.Comment}").ToList() ?? new()
            }).ToList();

            return Ok(fieldDtos);
        }

        
        
    }


}