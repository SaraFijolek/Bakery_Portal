using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class AdminRolePermissionsService : IAdminRolePermissionsService
    {
        private readonly AppDbContext _context;

        public AdminRolePermissionsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdminRolePermission>> GetAllAdminRolePermissionsAsync()
        {
            return await _context.AdminRolePermissions
                .Include(rp => rp.AdminPermission)
                .ToListAsync();
        }

        public async Task<AdminRolePermission?> GetAdminRolePermissionByIdAsync(int id)
        {
            return await _context.AdminRolePermissions
                .Include(rp => rp.AdminPermission)
                .FirstOrDefaultAsync(rp => rp.RolePermissionId == id);
        }

        public async Task<AdminRolePermission> CreateAdminRolePermissionAsync(AdminRolePermission rolePermission)
        {
            _context.AdminRolePermissions.Add(rolePermission);
            await _context.SaveChangesAsync();
            return rolePermission;
        }

        public async Task<bool> UpdateAdminRolePermissionAsync(int id, AdminRolePermission rolePermission)
        {
            if (id != rolePermission.RolePermissionId)
            {
                return false;
            }

            _context.Entry(rolePermission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdminRolePermissionExistsAsync(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAdminRolePermissionAsync(int id)
        {
            var rolePermission = await _context.AdminRolePermissions.FindAsync(id);
            if (rolePermission == null)
            {
                return false;
            }

            _context.AdminRolePermissions.Remove(rolePermission);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdminRolePermissionExistsAsync(int id)
        {
            return await _context.AdminRolePermissions.AnyAsync(e => e.RolePermissionId == id);
        }
    }
}
