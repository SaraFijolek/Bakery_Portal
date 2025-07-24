using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
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

        // Metody DTO
        public async Task<IEnumerable<AdminPermissionListItemDto>> GetAllAdminPermissionsDtoAsync()
        {
            return await _context.AdminPermissions
                .Select(p => new AdminPermissionListItemDto
                {
                    PermissionId = p.PermissionId,
                    Name = p.Name,
                    Category = p.Category
                })
                .ToListAsync();
        }

        public async Task<AdminPermissionResponseDto?> GetAdminPermissionByIdDtoAsync(int id)
        {
            return await _context.AdminPermissions
                .Where(p => p.PermissionId == id)
                .Select(p => new AdminPermissionResponseDto
                {
                    PermissionId = p.PermissionId,
                    Name = p.Name,
                    Description = p.Description,
                    Category = p.Category
                })
                .FirstOrDefaultAsync();
        }

        public async Task<AdminPermissionResponseDto> CreateAdminPermissionAsync(CreateAdminPermissionDto createDto)
        {
            var permission = new AdminPermission
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Category = createDto.Category
            };

            _context.AdminPermissions.Add(permission);
            await _context.SaveChangesAsync();

            return new AdminPermissionResponseDto
            {
                PermissionId = permission.PermissionId,
                Name = permission.Name,
                Description = permission.Description,
                Category = permission.Category
            };
        }

        public async Task<bool> UpdateAdminPermissionAsync(int id, UpdateAdminPermissionDto updateDto)
        {
            var existingPermission = await _context.AdminPermissions.FindAsync(id);
            if (existingPermission == null)
                return false;

            existingPermission.Name = updateDto.Name;
            existingPermission.Description = updateDto.Description;
            existingPermission.Category = updateDto.Category;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdminPermissionExistsAsync(id))
                    return false;
                throw;
            }
        }

        public async Task<bool> DeleteAdminPermissionAsync(int id)
        {
            var permission = await _context.AdminPermissions.FindAsync(id);
            if (permission == null)
                return false;

            _context.AdminPermissions.Remove(permission);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdminPermissionExistsAsync(int id)
        {
            return await _context.AdminPermissions.AnyAsync(e => e.PermissionId == id);
        }

        // Oryginalne metody (zachowane dla kompatybilności wstecznej)
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
                return false;

            _context.Entry(permission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdminPermissionExistsAsync(id))
                    return false;
                throw;
            }
        }
    }
}
