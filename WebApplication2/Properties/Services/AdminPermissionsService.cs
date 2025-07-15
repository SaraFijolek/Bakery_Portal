using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class AdminPermissionsService : IAdminPermissionsService
    {
        private readonly AppDbContext _context;

        public AdminPermissionsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdminPermission>> GetAllAdminPermissionsAsync()
        {
            return await _context.AdminPermissions.ToListAsync();
        }

        public async Task<AdminPermission?> GetAdminPermissionByIdAsync(int id)
        {
            return await _context.AdminPermissions.FindAsync(id);
        }

        public async Task<AdminPermission> CreateAdminPermissionAsync(AdminPermission permission)
        {
            _context.AdminPermissions.Add(permission);
            await _context.SaveChangesAsync();
            return permission;
        }

        public async Task<bool> UpdateAdminPermissionAsync(int id, AdminPermission permission)
        {
            if (id != permission.PermissionId)
            {
                return false;
            }

            _context.Entry(permission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdminPermissionExistsAsync(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAdminPermissionAsync(int id)
        {
            var permission = await _context.AdminPermissions.FindAsync(id);
            if (permission == null)
            {
                return false;
            }

            _context.AdminPermissions.Remove(permission);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdminPermissionExistsAsync(int id)
        {
            return await _context.AdminPermissions.AnyAsync(e => e.PermissionId == id);
        }
    }
}
