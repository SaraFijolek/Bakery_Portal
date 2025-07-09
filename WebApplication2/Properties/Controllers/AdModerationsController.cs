using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdModerationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdModerationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AdModerations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdModeration>>> GetAdModerations()
        {
            return await _context.AdModerations
                .Include(m => m.Ad)
                .Include(m => m.Admin)
                .ToListAsync();
        }

        // GET: api/AdModerations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdModeration>> GetAdModeration(int id)
        {
            var moderation = await _context.AdModerations
                .Include(m => m.Ad)
                .Include(m => m.Admin)
                .FirstOrDefaultAsync(m => m.ModerationId == id);

            if (moderation == null)
                return NotFound();

            return moderation;
        }

        // POST: api/AdModerations
        [HttpPost]
        public async Task<ActionResult<AdModeration>> CreateAdModeration(AdModeration moderation)
        {
            _context.AdModerations.Add(moderation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdModeration), new { id = moderation.ModerationId }, moderation);
        }

        // PUT: api/AdModerations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdModeration(int id, AdModeration moderation)
        {
            if (id != moderation.ModerationId)
                return BadRequest();

            _context.Entry(moderation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdModerationExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/AdModerations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdModeration(int id)
        {
            var moderation = await _context.AdModerations.FindAsync(id);
            if (moderation == null)
                return NotFound();

            _context.AdModerations.Remove(moderation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdModerationExists(int id)
        {
            return _context.AdModerations.Any(e => e.ModerationId == id);
        }
    }
}
