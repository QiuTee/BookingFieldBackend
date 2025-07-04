using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FieldBookingAPI.Data;
using FieldBookingAPI.DTOs;
using FieldBookingAPI.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using FieldBookingAPI.Migrations;
using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Http.Connections;
using System.Runtime.InteropServices;

namespace FieldBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductInfController : ControllerBase
    {
        public readonly AppDbContext _context;

        public ProductInfController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("createPos")]
        public async Task<IActionResult> CreateProductInf([FromBody] ProductInfDto dto)
        {
            var productInf = new ProductInf
            {
                Name = dto.Name,
                Price = dto.Price,
                Category = dto.Category,
                ImageUrl = dto.ImageUrl,
                Description = dto.Description,
                Available = dto.Available
            };
            _context.ProductInfs.Add(productInf);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "Product created successfully",
                productId = productInf.Id,
            });
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllProductInf()
        {
            var products = await _context.ProductInfs.ToListAsync();
            var productDtos = products.Select(product => new ProductInfDto
            {
                Name = product.Name,
                Price = product.Price,
                Category = product.Category,
                ImageUrl = product.ImageUrl,
                Description = product.Description,
                Available = product.Available
            }).ToList() ?? new();
            return Ok(productDtos);
        }
            
    }
}