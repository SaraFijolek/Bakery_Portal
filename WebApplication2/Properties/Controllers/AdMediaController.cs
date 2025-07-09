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
    public class AdMediaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdMediaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AdMedia
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admedia>>> GetAdMedia()
        {
            return await _context.Admedias
                .Include(m => m.Ad)
                .ToListAsync();
        }

        // GET: api/AdMedia/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Admedia>> GetAdMedia(int id)
        {
            var adMedia = await _context.Admedias
                .Include(m => m.Ad)
                .FirstOrDefaultAsync(m => m.MediaId == id);

            if (adMedia == null)
            {
                return NotFound();
            }

            return adMedia;
        }

        // POST: api/AdMedia
        [HttpPost]
        public async Task<ActionResult<Admedia>> CreateAdMedia(Admedia adMedia)
        {
            _context.Admedias.Add(adMedia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdMedia), new { id = adMedia.MediaId }, adMedia);
        }

        // PUT: api/AdMedia/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdMedia(int id, Admedia adMedia)
        {
            if (id != adMedia.MediaId)
            {
                return BadRequest();
            }

            _context.Entry(adMedia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdMediaExists(id))
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

        // DELETE: api/AdMedia/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdMedia(int id)
        {
            var adMedia = await _context.Admedias.FindAsync(id);
            if (adMedia == null)
            {
                return NotFound();
            }

            _context.Admedias.Remove(adMedia);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdMediaExists(int id)
        {
            return _context.Admedias.Any(e => e.MediaId == id);
        }
    }
}
