using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;
using WebApplication2.DTO;

namespace WebApplication2.Properties.Services
{
    public class AdminsService : IAdminsService
    {
        private readonly AppDbContext _context;

        public AdminsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdminDto>> GetAllAdminsAsync()
        {
            var admins = await _context.Admins
                .Include(a => a.AdminSessions)
                .Include(a => a.AdminAuditLogs)
                .Include(a => a.UserModerations)
                .Include(a => a.AdModerations)
                .Include(a => a.ReportedContents)
                .ToListAsync();
            return admins.Select(a => MapToAdminsDto(a));
        }

        public async Task<AdminDto?> GetAdminByIdAsync(int id)
        {
            var admin = await _context.Admins
                .Include(a => a.AdminSessions)
                .Include(a => a.AdminAuditLogs)
                .Include(a => a.UserModerations)
                .Include(a => a.AdModerations)
                .Include(a => a.ReportedContents)
                .FirstOrDefaultAsync(a => a.AdminId == id);
            return admin == null ? null : MapToAdminsDto(admin);
        }

        public async Task<AdminDto> CreateAdminAsync(CreateAdminDto createDto)
        {
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
            return MapToAdminsDto(admin);
        }

        public async Task<bool> UpdateAdminAsync(int id, UpdateAdminDto updateDto)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return false;
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
            return true;
        }

        public async Task<bool> DeleteAdminAsync(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return false;
            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdminExistsAsync(int id)
        {
            return await _context.Admins.AnyAsync(e => e.AdminId == id);
        }

        public async Task<IEnumerable<AdminListDto>> GetAllAdminsListAsync()
        {
            var admins = await _context.Admins.ToListAsync();
            return admins.Select(a => new AdminListDto
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
        }

        public async Task<AdminProfileDto?> GetAdminProfileAsync(int id)
        {
            var a = await _context.Admins.FindAsync(id);
            if (a == null) return null;
            return new AdminProfileDto
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
        }

        public async Task<IEnumerable<AdminDto>> GetActiveAdminsAsync()
        {
            var admins = await _context.Admins.Where(a => a.IsActive).ToListAsync();
            return admins.Select(a => MapToAdminsDto(a));
        }

        public async Task<IEnumerable<AdminDto>> GetAdminsByRoleAsync(string role)
        {
            var admins = await _context.Admins.Where(a => a.Role == role).ToListAsync();
            return admins.Select(a => MapToAdminsDto(a));
        }

        public async Task<bool> ChangeAdminPasswordAsync(int id, ChangeAdminPasswordDto changePasswordDto)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return false;
         
            admin.PasswordHash = changePasswordDto.NewPasswordHash;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Setup2FAAsync(int id, Setup2FADto setup2FADto)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return false;
            admin.TwoFactorEnabled = setup2FADto.TwoFactorEnabled;
            admin.TwoFactorSecret = setup2FADto.TwoFactorSecret;
            admin.BackupCodes = setup2FADto.BackupCodes;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AdminDto?> GetAdminByEmailAsync(string email)
        {
            var a = await _context.Admins.FirstOrDefaultAsync(x => x.Email == email);
            if (a == null) return null;
            return new AdminDto
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
