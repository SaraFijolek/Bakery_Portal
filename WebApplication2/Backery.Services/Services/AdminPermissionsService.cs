using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Properties.Services
{
    // ResultService class
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static ServiceResult<T> GoodResult(
            string message,
            int statusCode,
            T? data = default) => new ServiceResult<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = true,
                Data = data
            };

        public static ServiceResult<T> BadResult(
            string message,
            int statusCode,
            List<string>? errors = null,
            T? data = default) => new ServiceResult<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = false,
                Errors = errors ?? new List<string>(),
                Data = data
            };
    }

    public class AdminPermissionsService : IAdminPermissionsService
    {
        private readonly AppDbContext _context;

        public AdminPermissionsService(AppDbContext context)
        {
            _context = context;
        }

        // Metody DTO - zaktualizowane z wzorcem ResultService
        public async Task<ServiceResult<IEnumerable<AdminPermissionListItemDto>>> GetAllAdminPermissionsDtoAsync()
        {
            try
            {
                var permissions = await _context.AdminPermissions
                    .Select(p => new AdminPermissionListItemDto
                    {
                        PermissionId = p.PermissionId,
                        Name = p.Name,
                        Category = p.Category
                    })
                    .ToListAsync();

                return ServiceResult<IEnumerable<AdminPermissionListItemDto>>.GoodResult(
                    "Uprawnienia zostały pobrane pomyślnie",
                    200,
                    permissions
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<AdminPermissionListItemDto>>.BadResult(
                    "Błąd podczas pobierania uprawnień",
                    500,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<AdminPermissionResponseDto>> GetAdminPermissionByIdDtoAsync(int id)
        {
            try
            {
                var permission = await _context.AdminPermissions
                    .Where(p => p.PermissionId == id)
                    .Select(p => new AdminPermissionResponseDto
                    {
                        PermissionId = p.PermissionId,
                        Name = p.Name,
                        Description = p.Description,
                        Category = p.Category
                    })
                    .FirstOrDefaultAsync();

                if (permission == null)
                {
                    return ServiceResult<AdminPermissionResponseDto>.BadResult(
                        "Nie znaleziono uprawnienia o podanym ID",
                        404
                    );
                }

                return ServiceResult<AdminPermissionResponseDto>.GoodResult(
                    "Uprawnienie zostało pobrane pomyślnie",
                    200,
                    permission
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<AdminPermissionResponseDto>.BadResult(
                    "Błąd podczas pobierania uprawnienia",
                    500,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<AdminPermissionResponseDto>> CreateAdminPermissionAsync(CreateAdminPermissionDto createDto)
        {
            try
            {
                var permission = new AdminPermission
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    Category = createDto.Category
                };

                _context.AdminPermissions.Add(permission);
                await _context.SaveChangesAsync();

                var responseDto = new AdminPermissionResponseDto
                {
                    PermissionId = permission.PermissionId,
                    Name = permission.Name,
                    Description = permission.Description,
                    Category = permission.Category
                };

                return ServiceResult<AdminPermissionResponseDto>.GoodResult(
                    "Uprawnienie zostało utworzone pomyślnie",
                    201,
                    responseDto
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<AdminPermissionResponseDto>.BadResult(
                    "Błąd podczas tworzenia uprawnienia",
                    500,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<object>> UpdateAdminPermissionAsync(int id, UpdateAdminPermissionDto updateDto)
        {
            try
            {
                var existingPermission = await _context.AdminPermissions.FindAsync(id);
                if (existingPermission == null)
                {
                    return ServiceResult<object>.BadResult(
                        "Nie znaleziono uprawnienia o podanym ID",
                        404
                    );
                }

                existingPermission.Name = updateDto.Name;
                existingPermission.Description = updateDto.Description;
                existingPermission.Category = updateDto.Category;

                await _context.SaveChangesAsync();

                return ServiceResult<object>.GoodResult(
                    "Uprawnienie zostało zaktualizowane pomyślnie",
                    204
                );
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdminPermissionExistsAsync(id))
                {
                    return ServiceResult<object>.BadResult(
                        "Nie znaleziono uprawnienia o podanym ID",
                        404
                    );
                }

                return ServiceResult<object>.BadResult(
                    "Błąd współbieżności podczas aktualizacji uprawnienia",
                    409
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.BadResult(
                    "Błąd podczas aktualizacji uprawnienia",
                    500,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<object>> DeleteAdminPermissionAsync(int id)
        {
            try
            {
                var permission = await _context.AdminPermissions.FindAsync(id);
                if (permission == null)
                {
                    return ServiceResult<object>.BadResult(
                        "Nie znaleziono uprawnienia o podanym ID",
                        404
                    );
                }

                _context.AdminPermissions.Remove(permission);
                await _context.SaveChangesAsync();

                return ServiceResult<object>.GoodResult(
                    "Uprawnienie zostało usunięte pomyślnie",
                    204
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.BadResult(
                    "Błąd podczas usuwania uprawnienia",
                    500,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<bool> AdminPermissionExistsAsync(int id)
        {
            return await _context.AdminPermissions.AnyAsync(e => e.PermissionId == id);
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