using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PersonalDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PersonalDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonalDetails>>> GetPersonalDetails()
        {
            return await _context.PersonalDetails.ToListAsync();
        }

        // GET: api/PersonalDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonalDetails>> GetPersonalDetails(int id)
        {
            var personalDetails = await _context.PersonalDetails.FindAsync(id);

            if (personalDetails == null)
            {
                return NotFound();
            }

            return personalDetails;
        }

        // PUT: api/PersonalDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersonalDetails(int id, PersonalDetails personalDetails)
        {
            if (id != personalDetails.PersonId)
            {
                return BadRequest();
            }

            _context.Entry(personalDetails).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonalDetailsExists(id))
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

        // POST: api/PersonalDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PersonalDetails>> PostPersonalDetails([FromForm] int personId, string name, long mobileNumber, string aadhar, string address, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var personalDetails = new PersonalDetails
            {
                Name = name,
                MobileNumber = mobileNumber,
                Aadhar = aadhar,
                Address = address,
                UserId = userId

            };
            _context.PersonalDetails.Add(personalDetails);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPersonalDetails", new { id = personalDetails.PersonId }, personalDetails);
        }

        // DELETE: api/PersonalDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersonalDetails(int id)
        {
            var personalDetails = await _context.PersonalDetails.FindAsync(id);
            if (personalDetails == null)
            {
                return NotFound();
            }

            _context.PersonalDetails.Remove(personalDetails);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonalDetailsExists(int id)
        {
            return _context.PersonalDetails.Any(e => e.PersonId == id);
        }
    }
}
