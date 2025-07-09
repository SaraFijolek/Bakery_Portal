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
    public class AdminAuditLogsControllers : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminAuditLogsControllers(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AdminAuditLogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminAuditLog>>> GetAdminAuditLogs()
        {
            return await _context.AdminAuditLogs
                .Include(log => log.Admin)
                .ToListAsync();
        }

        // GET: api/AdminAuditLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminAuditLog>> GetAdminAuditLog(int id)
        {
            var log = await _context.AdminAuditLogs
                .Include(l => l.Admin)
                .FirstOrDefaultAsync(l => l.LogId == id);

            if (log == null)
            {
                return NotFound();
            }

            return log;
        }

        // POST: api/AdminAuditLogs
        [HttpPost]
        public async Task<ActionResult<AdminAuditLog>> CreateAdminAuditLog(AdminAuditLog log)
        {
            _context.AdminAuditLogs.Add(log);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdminAuditLog), new { id = log.LogId }, log);
        }

        // PUT: api/AdminAuditLogs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdminAuditLog(int id, AdminAuditLog log)
        {
            if (id != log.LogId)
            {
                return BadRequest();
            }

            _context.Entry(log).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminAuditLogExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/AdminAuditLogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminAuditLog(int id)
        {
            var log = await _context.AdminAuditLogs.FindAsync(id);
            if (log == null)
            {
                return NotFound();
            }

            _context.AdminAuditLogs.Remove(log);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdminAuditLogExists(int id)
        {
            return _context.AdminAuditLogs.Any(e => e.LogId == id);
        }
    }
}
