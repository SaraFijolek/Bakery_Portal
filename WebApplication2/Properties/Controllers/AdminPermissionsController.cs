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
    public class AdminPermissionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminPermissionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AdminPermissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminPermission>>> GetAdminPermissions()
        {
            return await _context.AdminPermissions.ToListAsync();
        }

        // GET: api/AdminPermissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminPermission>> GetAdminPermission(int id)
        {
            var permission = await _context.AdminPermissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound();
            }

            return permission;
        }

        // POST: api/AdminPermissions
        [HttpPost]
        public async Task<ActionResult<AdminPermission>> CreateAdminPermission(AdminPermission permission)
        {
            _context.AdminPermissions.Add(permission);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdminPermission), new { id = permission.PermissionId }, permission);
        }

        // PUT: api/AdminPermissions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdminPermission(int id, AdminPermission permission)
        {
            if (id != permission.PermissionId)
            {
                return BadRequest();
            }

            _context.Entry(permission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminPermissionExists(id))
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

        // DELETE: api/AdminPermissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminPermission(int id)
        {
            var permission = await _context.AdminPermissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound();
            }

            _context.AdminPermissions.Remove(permission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdminPermissionExists(int id)
        {
            return _context.AdminPermissions.Any(e => e.PermissionId == id);
        }
    }
}
