using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> GetProducts(int id)
        {
            var products = await _context.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();
            }

            return products;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducts(int id, Products products)
        {
            if (id != products.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(products).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Products>> PostProducts(int userId,[FromForm] Products products)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var imagePath = await SaveFile(products.ImagePath);
            var videoPath = await SaveFile(products.VideoPath);

            var product = new Products
            {
                Title = products.Title,
                Description = products.Description,
                Category = products.Category,
                Condition = products.Condition,
                StartingPrice = products.StartingPrice,
                StartingDate = products.StartingDate,
                EndingDate = products.EndingDate,
                Status = products.Status,
                SellerId = user.UserId,
                Image = imagePath,
                Video = videoPath,
                Users = user

            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducts", new { id = product.ProductId }, product);
        }
        private async Task<string> SaveFile(IFormFile file) 
        { 
            if (file == null || file.Length == 0) 
                return null; 
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads"); 
            if (!Directory.Exists(uploadsFolder)) 
                Directory.CreateDirectory(uploadsFolder); 
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); 
            var filePath = Path.Combine(uploadsFolder, fileName); 
            using (var stream = new FileStream(filePath, FileMode.Create)) 
            { 
                await file.CopyToAsync(stream); 
            } 
            return Path.Combine("uploads", fileName); 

        } 

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducts(int id)
        {
            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }

            _context.Products.Remove(products);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
