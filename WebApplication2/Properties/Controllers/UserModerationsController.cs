using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserModerationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserModerationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/UserModerations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModeration>>> GetUserModerations()
        {
            return await _context.UserModerations
                .Include(um => um.User)
                .Include(um => um.Admin)
                .ToListAsync();
        }

        // GET: api/UserModerations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModeration>> GetUserModeration(int id)
        {
            var moderation = await _context.UserModerations
                .Include(um => um.User)
                .Include(um => um.Admin)
                .FirstOrDefaultAsync(um => um.ModerationId == id);

            if (moderation == null)
                return NotFound();

            return moderation;
        }

        // POST: api/UserModerations
        [HttpPost]
        public async Task<ActionResult<UserModeration>> CreateUserModeration(UserModeration moderation)
        {
            _context.UserModerations.Add(moderation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserModeration), new { id = moderation.ModerationId }, moderation);
        }

        // PUT: api/UserModerations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserModeration(int id, UserModeration moderation)
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
                if (!UserModerationExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/UserModerations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserModeration(int id)
        {
            var moderation = await _context.UserModerations.FindAsync(id);
            if (moderation == null)
                return NotFound();

            _context.UserModerations.Remove(moderation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserModerationExists(int id)
        {
            return _context.UserModerations.Any(e => e.ModerationId == id);
        }
    }
}
