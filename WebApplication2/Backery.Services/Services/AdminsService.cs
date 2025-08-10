using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;
using WebApplication2.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Properties.Services
{
    public class AdminService<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static AdminService<T> GoodResult(
            string message,
            int statusCode,
            T? data = default)
            => new AdminService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = true,
                Data = data
            };

        public static AdminService<T> BadResult(
            string message,
            int statusCode,
            List<string>? errors = null,
            T? data = default)
            => new AdminService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = false,
                Errors = errors ?? new List<string>(),
                Data = data
            };
    }

    public class AdminsService : IAdminsService
    {
        private readonly AppDbContext _context;

        public AdminsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdminService<IEnumerable<AdminDto>>> GetAllAdminsAsync()
        {
            try
            {
                var admins = await _context.Admins
                    .Include(a => a.AdminSessions)
                    .Include(a => a.AdminAuditLogs)
                    .Include(a => a.UserModerations)
                    .Include(a => a.AdModerations)
                    .Include(a => a.ReportedContents)
                    .ToListAsync();

                var adminDtos = admins.Select(a => MapToAdminsDto(a));

                return AdminService<IEnumerable<AdminDto>>.GoodResult(
                    "Admins retrieved successfully",
                    200,
                    adminDtos);
            }
            catch (Exception ex)
            {
                return AdminService<IEnumerable<AdminDto>>.BadResult(
                    "Failed to retrieve admins",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdminService<AdminDto>> GetAdminByIdAsync(int id)
        {
            try
            {
                var admin = await _context.Admins
                    .Include(a => a.AdminSessions)
                    .Include(a => a.AdminAuditLogs)
                    .Include(a => a.UserModerations)
                    .Include(a => a.AdModerations)
                    .Include(a => a.ReportedContents)
                    .FirstOrDefaultAsync(a => a.AdminId == id);

                if (admin == null)
                {
                    return AdminService<AdminDto>.BadResult(
                        $"Admin with ID {id} not found",
                        404);
                }

                return AdminService<AdminDto>.GoodResult(
                    "Admin retrieved successfully",
                    200,
                    MapToAdminsDto(admin));
            }
            catch (Exception ex)
            {
                return AdminService<AdminDto>.BadResult(
                    "Failed to retrieve admin",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdminService<AdminDto>> CreateAdminAsync(CreateAdminDto createDto)
        {
            try
            {
                // Validate input
                var errors = new List<string>();
                if (string.IsNullOrWhiteSpace(createDto.Email))
                    errors.Add("Email is required");
                if (string.IsNullOrWhiteSpace(createDto.Name))
                    errors.Add("Name is required");
                if (string.IsNullOrWhiteSpace(createDto.PasswordHash))
                    errors.Add("Password hash is required");

                // Check if email already exists
                var emailExists = await _context.Admins.AnyAsync(a => a.Email == createDto.Email);
                if (emailExists)
                    errors.Add("Admin with this email already exists");

                if (errors.Any())
                {
                    return AdminService<AdminDto>.BadResult(
                        "Validation failed",
                        400,
                        errors);
                }

                var admin = new Admin
                {
                    Email = createDto.Email,
                    PasswordHash = createDto.PasswordHash,
                    Name = createDto.Name,
                    Phone = createDto.Phone,
                    AvatarUrl = createDto.AvatarUrl,
                    Role = createDto.Role,
                    IsActive = createDto.IsActive,
                    TwoFactorEnabled = createDto.TwoFactorEnabled,
                    TwoFactorSecret = createDto.TwoFactorSecret,
                    BackupCodes = createDto.BackupCodes,
                    CreatedAt = DateTime.Now
                };

                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();

                return AdminService<AdminDto>.GoodResult(
                    "Admin created successfully",
                    201,
                    MapToAdminsDto(admin));
            }
            catch (Exception ex)
            {
                return AdminService<AdminDto>.BadResult(
                    "Failed to create admin",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdminService<AdminDto>> UpdateAdminAsync(int id, UpdateAdminDto updateDto)
        {
            try
            {
                var admin = await _context.Admins.FindAsync(id);
                if (admin == null)
                {
                    return AdminService<AdminDto>.BadResult(
                        $"Admin with ID {id} not found",
                        404);
                }

                // Validate input
                var errors = new List<string>();
                if (string.IsNullOrWhiteSpace(updateDto.Email))
                    errors.Add("Email is required");
                if (string.IsNullOrWhiteSpace(updateDto.Name))
                    errors.Add("Name is required");

                // Check if email already exists for another admin
                var emailExists = await _context.Admins.AnyAsync(a => a.Email == updateDto.Email && a.AdminId != id);
                if (emailExists)
                    errors.Add("Another admin with this email already exists");

                if (errors.Any())
                {
                    return AdminService<AdminDto>.BadResult(
                        "Validation failed",
                        400,
                        errors);
                }

                admin.Email = updateDto.Email;
                if (!string.IsNullOrEmpty(updateDto.PasswordHash))
                    admin.PasswordHash = updateDto.PasswordHash;
                admin.Name = updateDto.Name;
                admin.Phone = updateDto.Phone;
                admin.AvatarUrl = updateDto.AvatarUrl;
                admin.Role = updateDto.Role;
                admin.IsActive = updateDto.IsActive;
                admin.TwoFactorEnabled = updateDto.TwoFactorEnabled;
                admin.TwoFactorSecret = updateDto.TwoFactorSecret;
                admin.BackupCodes = updateDto.BackupCodes;

                await _context.SaveChangesAsync();

                return AdminService<AdminDto>.GoodResult(
                    "Admin updated successfully",
                    200,
                    MapToAdminsDto(admin));
            }
            catch (Exception ex)
            {
                return AdminService<AdminDto>.BadResult(
                    "Failed to update admin",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdminService<bool>> DeleteAdminAsync(int id)
        {
            try
            {
                var admin = await _context.Admins.FindAsync(id);
                if (admin == null)
                {
                    return AdminService<bool>.BadResult(
                        $"Admin with ID {id} not found",
                        404,
                        data: false);
                }

                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();

                return AdminService<bool>.GoodResult(
                    "Admin deleted successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return AdminService<bool>.BadResult(
                    "Failed to delete admin",
                    500,
                    new List<string> { ex.Message },
                    false);
            }
        }

        public async Task<bool> AdminExistsAsync(int id)
        {
            return await _context.Admins.AnyAsync(e => e.AdminId == id);
        }

        public async Task<AdminService<IEnumerable<AdminListDto>>> GetAllAdminsListAsync()
        {
            try
            {
                var admins = await _context.Admins.ToListAsync();
                var adminListDtos = admins.Select(a => new AdminListDto
                {
                    AdminId = a.AdminId,
                    Email = a.Email,
                    Name = a.Name,
                    Phone = a.Phone,
                    AvatarUrl = a.AvatarUrl,
                    Role = a.Role,
                    IsActive = a.IsActive,
                    TwoFactorEnabled = a.TwoFactorEnabled,
                    CreatedAt = a.CreatedAt,
                    LastLogin = a.LastLogin,
                    FailedLoginAttempts = a.FailedLoginAttempts,
                    LockedUntil = a.LockedUntil,
                    SessionsCount = a.AdminSessions?.Count ?? 0,
                    AuditLogsCount = a.AdminAuditLogs?.Count ?? 0,
                    UserModerationsCount = a.UserModerations?.Count ?? 0,
                    AdModerationsCount = a.AdModerations?.Count ?? 0,
                    ReportedContentsCount = a.ReportedContents?.Count ?? 0
                });

                return AdminService<IEnumerable<AdminListDto>>.GoodResult(
                    "Admin list retrieved successfully",
                    200,
                    adminListDtos);
            }
            catch (Exception ex)
            {
                return AdminService<IEnumerable<AdminListDto>>.BadResult(
                    "Failed to retrieve admin list",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdminService<AdminProfileDto>> GetAdminProfileAsync(int id)
        {
            try
            {
                var a = await _context.Admins.FindAsync(id);
                if (a == null)
                {
                    return AdminService<AdminProfileDto>.BadResult(
                        $"Admin with ID {id} not found",
                        404);
                }

                var profileDto = new AdminProfileDto
                {
                    AdminId = a.AdminId,
                    Email = a.Email,
                    Name = a.Name,
                    Phone = a.Phone,
                    AvatarUrl = a.AvatarUrl,
                    Role = a.Role,
                    IsActive = a.IsActive,
                    TwoFactorEnabled = a.TwoFactorEnabled,
                    CreatedAt = a.CreatedAt,
                    LastLogin = a.LastLogin
                };

                return AdminService<AdminProfileDto>.GoodResult(
                    "Admin profile retrieved successfully",
                    200,
                    profileDto);
            }
            catch (Exception ex)
            {
                return AdminService<AdminProfileDto>.BadResult(
                    "Failed to retrieve admin profile",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdminService<IEnumerable<AdminDto>>> GetActiveAdminsAsync()
        {
            try
            {
                var admins = await _context.Admins.Where(a => a.IsActive).ToListAsync();
                var adminDtos = admins.Select(a => MapToAdminsDto(a));

                return AdminService<IEnumerable<AdminDto>>.GoodResult(
                    "Active admins retrieved successfully",
                    200,
                    adminDtos);
            }
            catch (Exception ex)
            {
                return AdminService<IEnumerable<AdminDto>>.BadResult(
                    "Failed to retrieve active admins",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdminService<IEnumerable<AdminDto>>> GetAdminsByRoleAsync(string role)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(role))
                {
                    return AdminService<IEnumerable<AdminDto>>.BadResult(
                        "Role parameter is required",
                        400);
                }

                var admins = await _context.Admins.Where(a => a.Role == role).ToListAsync();
                var adminDtos = admins.Select(a => MapToAdminsDto(a));

                return AdminService<IEnumerable<AdminDto>>.GoodResult(
                    $"Admins with role '{role}' retrieved successfully",
                    200,
                    adminDtos);
            }
            catch (Exception ex)
            {
                return AdminService<IEnumerable<AdminDto>>.BadResult(
                    "Failed to retrieve admins by role",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdminService<bool>> ChangeAdminPasswordAsync(int id, ChangeAdminPasswordDto changePasswordDto)
        {
            try
            {
                var admin = await _context.Admins.FindAsync(id);
                if (admin == null)
                {
                    return AdminService<bool>.BadResult(
                        $"Admin with ID {id} not found",
                        404,
                        data: false);
                }

                if (string.IsNullOrWhiteSpace(changePasswordDto.NewPasswordHash))
                {
                    return AdminService<bool>.BadResult(
                        "New password hash is required",
                        400,
                        data: false);
                }

                admin.PasswordHash = changePasswordDto.NewPasswordHash;
                await _context.SaveChangesAsync();

                return AdminService<bool>.GoodResult(
                    "Admin password changed successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return AdminService<bool>.BadResult(
                    "Failed to change admin password",
                    500,
                    new List<string> { ex.Message },
                    false);
            }
        }

        public async Task<AdminService<bool>> Setup2FAAsync(int id, Setup2FADto setup2FADto)
        {
            try
            {
                var admin = await _context.Admins.FindAsync(id);
                if (admin == null)
                {
                    return AdminService<bool>.BadResult(
                        $"Admin with ID {id} not found",
                        404,
                        data: false);
                }

                admin.TwoFactorEnabled = setup2FADto.TwoFactorEnabled;
                admin.TwoFactorSecret = setup2FADto.TwoFactorSecret;
                admin.BackupCodes = setup2FADto.BackupCodes;
                await _context.SaveChangesAsync();

                return AdminService<bool>.GoodResult(
                    "2FA setup completed successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return AdminService<bool>.BadResult(
                    "Failed to setup 2FA",
                    500,
                    new List<string> { ex.Message },
                    false);
            }
        }

        public async Task<AdminService<AdminDto>> GetAdminByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return AdminService<AdminDto>.BadResult(
                        "Email parameter is required",
                        400);
                }

                var a = await _context.Admins.FirstOrDefaultAsync(x => x.Email == email);
                if (a == null)
                {
                    return AdminService<AdminDto>.BadResult(
                        $"Admin with email '{email}' not found",
                        404);
                }

                var adminDto = new AdminDto
                {
                    AdminId = a.AdminId,
                    Email = a.Email,
                    Name = a.Name,
                    Phone = a.Phone,
                    AvatarUrl = a.AvatarUrl,
                    Role = a.Role,
                    IsActive = a.IsActive,
                    TwoFactorEnabled = a.TwoFactorEnabled,
                    CreatedAt = a.CreatedAt,
                    LastLogin = a.LastLogin
                };

                return AdminService<AdminDto>.GoodResult(
                    "Admin retrieved by email successfully",
                    200,
                    adminDto);
            }
            catch (Exception ex)
            {
                return AdminService<AdminDto>.BadResult(
                    "Failed to retrieve admin by email",
                    500,
                    new List<string> { ex.Message });
            }
        }

        private AdminDto MapToAdminsDto(Admin admin)
        {
            return new AdminDto
            {
                AdminId = admin.AdminId,
                Email = admin.Email,
                Name = admin.Name,
                Phone = admin.Phone,
                AvatarUrl = admin.AvatarUrl,
                Role = admin.Role,
                IsActive = admin.IsActive,
                TwoFactorEnabled = admin.TwoFactorEnabled,
                CreatedAt = admin.CreatedAt,
                LastLogin = admin.LastLogin,
                FailedLoginAttempts = admin.FailedLoginAttempts,
                LockedUntil = admin.LockedUntil,
            };
        }
    }
}