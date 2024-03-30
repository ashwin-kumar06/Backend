using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public ProductsController(AppDbContext context, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration;
        }

        [HttpGet("BySeller/{sellerId}")]
        public async Task<ActionResult<IEnumerable<Products>>> GetBySellerId(int sellerId)
        {
            var products = await _context.Products.Where(p => p.SellerId == sellerId).ToListAsync();
            if (products == null || products.Count == 0)
            {
                return NotFound();
            }
            return Ok(products);
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            if (products == null || products.Count == 0)
            {
                return NoContent();
            }
            return products;
        }


        [HttpGet("{id}/Image")]
        public IActionResult GetImage(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound(); // User not found
            }

            // Construct the full path to the image file
            var imagePath = Path.Combine(_environment.WebRootPath, "images", product.Image);

            // Check if the image file exists
            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound(); // Image file not found
            }

            // Serve the image file
            return PhysicalFile(imagePath, "image/jpeg");
        }

        // GET: api/Products/5
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

        // PUT: api/Products/PrductStatus/5
        [HttpPut("ProductStatus/{productId}")]
        public async Task<IActionResult> PutByProductId(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(b => b.ProductId == productId);
            if (product == null)
            {
                return NotFound();
            }
            product.Status = "Close";
            _context.Entry(product).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductId))
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

        private bool ProductExists(int productId)
        {
            return _context.Products.Any(b => b.ProductId == productId);
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Products>> PostProducts(int userId, [FromForm] Products products, [FromServices] IHttpContextAccessor httpContextAccessor)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var image = $"{Guid.NewGuid()}_{products.ImagePath.FileName}";
            var imageuploadsFolder = Path.Combine(_environment.WebRootPath, "images");
            var imagefilePath = Path.Combine(imageuploadsFolder, image);
            using (var stream = new FileStream(imagefilePath, FileMode.Create))
            {
                await products.ImagePath.CopyToAsync(stream);
            }
            var today = DateTime.Now.ToString("M/d/yyyy");
            var product = new Products
            {
                Title = products.Title,
                Description = products.Description,
                Category = products.Category,
                Condition = products.Condition,
                StartingPrice = products.StartingPrice,
                StartingDate = today,
                EndingDate = products.EndingDate,
                Status = products.Status,
                SellerId = user.UserId,
                Image = image,
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            var imageUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}/images/{image}";
            return CreatedAtAction("GetProducts", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
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
