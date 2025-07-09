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
    public class AdminRolePermissionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminRolePermissionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AdminRolePermissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminRolePermission>>> GetAdminRolePermissions()
        {
            return await _context.AdminRolePermissions
                .Include(rp => rp.AdminPermission)
                .ToListAsync();
        }

        // GET: api/AdminRolePermissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminRolePermission>> GetAdminRolePermission(int id)
        {
            var rolePermission = await _context.AdminRolePermissions
                .Include(rp => rp.AdminPermission)
                .FirstOrDefaultAsync(rp => rp.RolePermissionId == id);

            if (rolePermission == null)
            {
                return NotFound();
            }

            return rolePermission;
        }

        // POST: api/AdminRolePermissions
        [HttpPost]
        public async Task<ActionResult<AdminRolePermission>> CreateAdminRolePermission(AdminRolePermission rolePermission)
        {
            _context.AdminRolePermissions.Add(rolePermission);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdminRolePermission), new { id = rolePermission.RolePermissionId }, rolePermission);
        }

        // PUT: api/AdminRolePermissions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdminRolePermission(int id, AdminRolePermission rolePermission)
        {
            if (id != rolePermission.RolePermissionId)
                return BadRequest();

            _context.Entry(rolePermission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminRolePermissionExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/AdminRolePermissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminRolePermission(int id)
        {
            var rolePermission = await _context.AdminRolePermissions.FindAsync(id);
            if (rolePermission == null)
                return NotFound();

            _context.AdminRolePermissions.Remove(rolePermission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdminRolePermissionExists(int id)
        {
            return _context.AdminRolePermissions.Any(e => e.RolePermissionId == id);
        }
    }
}
