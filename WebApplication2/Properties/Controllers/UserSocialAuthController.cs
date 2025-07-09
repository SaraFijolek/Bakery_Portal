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
    public class UserSocialAuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserSocialAuthController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/UserSocialAuth
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserSocialAuth>>> GetUserSocialAuths()
        {
            return await _context.UserSocialAuths
                .Include(ua => ua.User)
                .ToListAsync();
        }

        // GET: api/UserSocialAuth/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserSocialAuth>> GetUserSocialAuth(int id)
        {
            var auth = await _context.UserSocialAuths
                .Include(ua => ua.User)
                .FirstOrDefaultAsync(ua => ua.SocialAuthId == id);

            if (auth == null)
            {
                return NotFound();
            }
            return auth;
        }

        // POST: api/UserSocialAuth
        [HttpPost]
        public async Task<ActionResult<UserSocialAuth>> CreateUserSocialAuth(UserSocialAuth auth)
        {
            _context.UserSocialAuths.Add(auth);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserSocialAuth), new { id = auth.SocialAuthId }, auth);
        }

        // PUT: api/UserSocialAuth/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserSocialAuth(int id, UserSocialAuth auth)
        {
            if (id != auth.SocialAuthId)
            {
                return BadRequest();
            }

            _context.Entry(auth).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserSocialAuthExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/UserSocialAuth/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserSocialAuth(int id)
        {
            var auth = await _context.UserSocialAuths.FindAsync(id);
            if (auth == null)
            {
                return NotFound();
            }

            _context.UserSocialAuths.Remove(auth);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserSocialAuthExists(int id)
        {
            return _context.UserSocialAuths.Any(e => e.SocialAuthId == id);
        }
    }
}
