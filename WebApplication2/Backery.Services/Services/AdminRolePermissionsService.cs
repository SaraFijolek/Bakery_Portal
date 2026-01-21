using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
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

        public async Task<ResultService<IEnumerable<AdminRolePermissionListItemDto>>> GetAllAdminRolePermissionsDtoAsync()
        {
            try
            {
                var permissions = await _context.AdminRolePermissions
                    .Include(rp => rp.AdminPermission)
                    .Select(rp => new AdminRolePermissionListItemDto
                    {
                        RolePermissionId = rp.RolePermissionId,
                        Role = rp.Role,
                        PermissionId = rp.PermissionId,
                        PermissionName = rp.AdminPermission.Name,
                        PermissionCategory = rp.AdminPermission.Category
                    })
                    .ToListAsync();

                return ResultService<IEnumerable<AdminRolePermissionListItemDto>>.GoodResult(
                    "Admin role permissions retrieved successfully",
                    200,
                    permissions);
            }
            catch (Exception ex)
            {
                return ResultService<IEnumerable<AdminRolePermissionListItemDto>>.BadResult(
                    "Error occurred while retrieving admin role permissions",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ResultService<AdminRolePermissionResponseDto>> GetAdminRolePermissionByIdDtoAsync(int id)
        {
            try
            {
                var permission = await _context.AdminRolePermissions
                    .Include(rp => rp.AdminPermission)
                    .Where(rp => rp.RolePermissionId == id)
                    .Select(rp => new AdminRolePermissionResponseDto
                    {
                        RolePermissionId = rp.RolePermissionId,
                        Role = rp.Role,
                        PermissionId = rp.PermissionId,
                        AdminPermission = new AdminPermissionDto
                        {
                            PermissionId = rp.AdminPermission.PermissionId,
                            Name = rp.AdminPermission.Name,
                            Description = rp.AdminPermission.Description,
                            Category = rp.AdminPermission.Category
                        }
                    })
                    .FirstOrDefaultAsync();

                if (permission == null)
                {
                    return ResultService<AdminRolePermissionResponseDto>.BadResult(
                        $"Admin role permission with ID {id} not found",
                        404);
                }

                return ResultService<AdminRolePermissionResponseDto>.GoodResult(
                    "Admin role permission retrieved successfully",
                    200,
                    permission);
            }
            catch (Exception ex)
            {
                return ResultService<AdminRolePermissionResponseDto>.BadResult(
                    "Error occurred while retrieving admin role permission",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ResultService<AdminRolePermissionResponseDto>> CreateAdminRolePermissionAsync(CreateAdminRolePermissionDto createDto)
        {
            try
            {
                var rolePermission = new AdminRolePermission
                {
                    Role = createDto.Role,
                    PermissionId = createDto.PermissionId
                };

                _context.AdminRolePermissions.Add(rolePermission);
                await _context.SaveChangesAsync();

                var createdPermissionResult = await GetAdminRolePermissionByIdDtoAsync(rolePermission.RolePermissionId);

                if (createdPermissionResult.Success)
                {
                    return ResultService<AdminRolePermissionResponseDto>.GoodResult(
                        "Admin role permission created successfully",
                        201,
                        createdPermissionResult.Data);
                }
                else
                {
                    return ResultService<AdminRolePermissionResponseDto>.BadResult(
                        "Error occurred while retrieving created admin role permission",
                        500);
                }
            }
            catch (Exception ex)
            {
                return ResultService<AdminRolePermissionResponseDto>.BadResult(
                    "Error occurred while creating admin role permission",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ResultService<bool>> UpdateAdminRolePermissionAsync(int id, UpdateAdminRolePermissionDto updateDto)
        {
            try
            {
                var existingRolePermission = await _context.AdminRolePermissions.FindAsync(id);
                if (existingRolePermission == null)
                {
                    return ResultService<bool>.BadResult(
                        $"Admin role permission with ID {id} not found",
                        404);
                }

                existingRolePermission.Role = updateDto.Role;
                existingRolePermission.PermissionId = updateDto.PermissionId;

                try
                {
                    await _context.SaveChangesAsync();

                    return ResultService<bool>.GoodResult(
                        "Admin role permission updated successfully",
                        200,
                        true);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await AdminRolePermissionExistsAsync(id))
                    {
                        return ResultService<bool>.BadResult(
                            $"Admin role permission with ID {id} not found",
                            404);
                    }
                    throw;
                }
            }
            catch (Exception ex)
            {
                return ResultService<bool>.BadResult(
                    "Error occurred while updating admin role permission",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ResultService<bool>> DeleteAdminRolePermissionAsync(int id)
        {
            try
            {
                var rolePermission = await _context.AdminRolePermissions.FindAsync(id);
                if (rolePermission == null)
                {
                    return ResultService<bool>.BadResult(
                        $"Admin role permission with ID {id} not found",
                        404);
                }

                _context.AdminRolePermissions.Remove(rolePermission);
                await _context.SaveChangesAsync();

                return ResultService<bool>.GoodResult(
                    "Admin role permission deleted successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return ResultService<bool>.BadResult(
                    "Error occurred while deleting admin role permission",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<bool> AdminRolePermissionExistsAsync(int id)
        {
            return await _context.AdminRolePermissions.AnyAsync(e => e.RolePermissionId == id);
        }

        
        public async Task<ResultService<IEnumerable<AdminRolePermission>>> GetAllAdminRolePermissionsAsync()
        {
            try
            {
                var permissions = await _context.AdminRolePermissions
                    .Include(rp => rp.AdminPermission)
                    .ToListAsync();

                return ResultService<IEnumerable<AdminRolePermission>>.GoodResult(
                    "Admin role permissions retrieved successfully",
                    200,
                    permissions);
            }
            catch (Exception ex)
            {
                return ResultService<IEnumerable<AdminRolePermission>>.BadResult(
                    "Error occurred while retrieving admin role permissions",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ResultService<AdminRolePermission>> GetAdminRolePermissionByIdAsync(int id)
        {
            try
            {
                var permission = await _context.AdminRolePermissions
                    .Include(rp => rp.AdminPermission)
                    .FirstOrDefaultAsync(rp => rp.RolePermissionId == id);

                if (permission == null)
                {
                    return ResultService<AdminRolePermission>.BadResult(
                        $"Admin role permission with ID {id} not found",
                        404);
                }

                return ResultService<AdminRolePermission>.GoodResult(
                    "Admin role permission retrieved successfully",
                    200,
                    permission);
            }
            catch (Exception ex)
            {
                return ResultService<AdminRolePermission>.BadResult(
                    "Error occurred while retrieving admin role permission",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ResultService<AdminRolePermission>> CreateAdminRolePermissionAsync(AdminRolePermission rolePermission)
        {
            try
            {
                _context.AdminRolePermissions.Add(rolePermission);
                await _context.SaveChangesAsync();

                return ResultService<AdminRolePermission>.GoodResult(
                    "Admin role permission created successfully",
                    201,
                    rolePermission);
            }
            catch (Exception ex)
            {
                return ResultService<AdminRolePermission>.BadResult(
                    "Error occurred while creating admin role permission",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ResultService<bool>> UpdateAdminRolePermissionAsync(int id, AdminRolePermission rolePermission)
        {
            try
            {
                if (id != rolePermission.RolePermissionId)
                {
                    return ResultService<bool>.BadResult(
                        "ID mismatch between parameter and entity",
                        400);
                }

                _context.Entry(rolePermission).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();

                    return ResultService<bool>.GoodResult(
                        "Admin role permission updated successfully",
                        200,
                        true);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await AdminRolePermissionExistsAsync(id))
                    {
                        return ResultService<bool>.BadResult(
                            $"Admin role permission with ID {id} not found",
                            404);
                    }
                    throw;
                }
            }
            catch (Exception ex)
            {
                return ResultService<bool>.BadResult(
                    "Error occurred while updating admin role permission",
                    500,
                    new List<string> { ex.Message });
            }
        }
    }
}