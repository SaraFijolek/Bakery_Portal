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
    public class AdminSessionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminSessionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AdminSessions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminSession>>> GetAdminSessions()
        {
            return await _context.AdminSessions
                .Include(s => s.Admin)
                .ToListAsync();
        }

        // GET: api/AdminSessions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminSession>> GetAdminSession(int id)
        {
            var session = await _context.AdminSessions
                .Include(s => s.Admin)
                .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null)
            {
                return NotFound();
            }

            return session;
        }

        // POST: api/AdminSessions
        [HttpPost]
        public async Task<ActionResult<AdminSession>> CreateAdminSession(AdminSession session)
        {
            _context.AdminSessions.Add(session);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdminSession), new { id = session.SessionId }, session);
        }

        // PUT: api/AdminSessions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdminSession(int id, AdminSession session)
        {
            if (id != session.SessionId)
            {
                return BadRequest();
            }

            _context.Entry(session).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminSessionExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/AdminSessions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminSession(int id)
        {
            var session = await _context.AdminSessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            _context.AdminSessions.Remove(session);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdminSessionExists(int id)
        {
            return _context.AdminSessions.Any(e => e.SessionId == id);
        }
    }
}
