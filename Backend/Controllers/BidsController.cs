using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;
using Backend.Migrations;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BidsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public BidsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bid>>> GetBid()
        {
            return await _context.Bid.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Bid>> GetBid(int id)
        {
            var bid = await _context.Bid.FindAsync(id);

            if (bid == null)
            {
                return NotFound();
            }

            return bid;
        }

        [HttpGet("BidderId/{bidderId}")]
        public async Task<ActionResult<Bid>> GetByBidderId(int bidderId)
        {
            var bid = await _context.Bid.Where(b => b.BidderId == bidderId).ToListAsync();

            if (bid == null || bid.Count == 0)
            {
                return NotFound("Not found");
            }

            return Ok(bid);    

        }

        [HttpGet("ProductId/{productId}")]
        public async Task<ActionResult<Bid>> GetByProductId(int productId)
        {
            var bid = await _context.Bid.Where(b => b.ProductId == productId).ToListAsync();

            if (bid == null || bid.Count == 0)
            {
                return NotFound("Not found");
            }

            return Ok(bid);

        }

        // PUT: api/Bids/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBid(int id, Bid bid)
        {
            if (id != bid.BidId)
            {
                return BadRequest();
            }

            _context.Entry(bid).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BidExists(id))
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

        [HttpPut("BidStatus/{bidderId}")]
        public async Task<IActionResult> PutByBidder(int bidderId, int productId, int bidAmount)
        {
            var bid = await _context.Bid.FirstOrDefaultAsync(b => b.BidderId == bidderId && b.ProductId == productId && b.BidAmount == bidAmount);
            if (bid == null)
            {
                return NotFound();
            }
            bid.BidStatus = "Buy Now";
            _context.Entry(bid).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BidExists(bid.BidId))
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
        public async Task<ActionResult<Bid>> PostBid(int userId, int productId, int bidAmount)
        {
            var user = await _context.Users.FindAsync(userId);
            var product = await _context.Products.FindAsync(productId);

            if (user == null || product == null)
            {
                return NotFound("Data not found");
            }
            if (bidAmount > product.StartingPrice)
            { 
                product.StartingPrice = bidAmount;
                await _context.SaveChangesAsync();
            }
            var bidding = new Bid
            {
                ProductId = productId,
                BidderId = userId,
                BidAmount = bidAmount,
                TimeStamp = DateTime.UtcNow,
                BidStatus = "waiting"
                
            };

            _context.Bid.Add(bidding);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBid", new { id = bidding.BidId }, bidding);
        }

        // DELETE: api/Bids/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBid(int id)
        {
            var bid = await _context.Bid.FindAsync(id);
            if (bid == null)
            {
                return NotFound();
            }

            _context.Bid.Remove(bid);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BidExists(int id)
        {
            return _context.Bid.Any(e => e.BidId == id);
        }
    }
}
