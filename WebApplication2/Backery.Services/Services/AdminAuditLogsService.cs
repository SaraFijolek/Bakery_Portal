using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class AdminAuditLogsService : IAdminAuditLogsService
    {
        private readonly AppDbContext _context;

        public AdminAuditLogsService(AppDbContext context)
        {
            _context = context;
        }

        // Metody DTO
        public async Task<List<AdminAuditLogListItemDto>> GetAdminAuditLogsDtoAsync()
        {
            return await _context.AdminAuditLogs
                .Include(log => log.Admin)
                .Select(log => new AdminAuditLogListItemDto
                {
                    LogId = log.LogId,
                    AdminId = log.AdminId,
                    Action = log.Action,
                    TargetType = log.TargetType,
                    TargetId = log.TargetId,
                    Description = log.Description,
                    IpAddress = log.IpAddress,
                    CreatedAt = log.CreatedAt,
                    AdminUsername = log.Admin.Name,
                    AdminEmail = log.Admin.Email
                })
                .ToListAsync();
        }

        public async Task<AdminAuditLogResponseDto> GetAdminAuditLogByIdDtoAsync(int id)
        {
            return await _context.AdminAuditLogs
                .Include(l => l.Admin)
                .Where(l => l.LogId == id)
                .Select(log => new AdminAuditLogResponseDto
                {
                    LogId = log.LogId,
                    AdminId = log.AdminId,
                    Action = log.Action,
                    TargetType = log.TargetType,
                    TargetId = log.TargetId,
                    Description = log.Description,
                    IpAddress = log.IpAddress,
                    UserAgent = log.UserAgent,
                    CreatedAt = log.CreatedAt,
                    Admin = new AdminDto
                    {
                        AdminId = log.Admin.AdminId,
                        Username = log.Admin.Name,
                        Email = log.Admin.Email,
                      
                    }
                })
                .FirstOrDefaultAsync();
        }

        public async Task<AdminAuditLogResponseDto> CreateAdminAuditLogAsync(CreateAdminAuditLogDto createDto)
        {
            var log = new AdminAuditLog
            {
                AdminId = createDto.AdminId,
                Action = createDto.Action,
                TargetType = createDto.TargetType,
                TargetId = createDto.TargetId,
                Description = createDto.Description,
                IpAddress = createDto.IpAddress,
                UserAgent = createDto.UserAgent,
                CreatedAt = DateTime.Now
            };

            _context.AdminAuditLogs.Add(log);
            await _context.SaveChangesAsync();

            return await GetAdminAuditLogByIdDtoAsync(log.LogId);
        }

        public async Task<bool> UpdateAdminAuditLogAsync(int id, UpdateAdminAuditLogDto updateDto)
        {
            var existingLog = await _context.AdminAuditLogs.FindAsync(id);
            if (existingLog == null)
                return false;

            existingLog.AdminId = updateDto.AdminId;
            existingLog.Action = updateDto.Action;
            existingLog.TargetType = updateDto.TargetType;
            existingLog.TargetId = updateDto.TargetId;
            existingLog.Description = updateDto.Description;
            existingLog.IpAddress = updateDto.IpAddress;
            existingLog.UserAgent = updateDto.UserAgent;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdminAuditLogExistsAsync(id))
                    return false;
                throw;
            }
        }

        public async Task<bool> DeleteAdminAuditLogAsync(int id)
        {
            var log = await _context.AdminAuditLogs.FindAsync(id);
            if (log == null)
                return false;

            _context.AdminAuditLogs.Remove(log);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdminAuditLogExistsAsync(int id)
        {
            return await _context.AdminAuditLogs.AnyAsync(e => e.LogId == id);
        }

        // Oryginalne metody (zachowane dla kompatybilności wstecznej)
        public async Task<List<AdminAuditLog>> GetAdminAuditLogsAsync()
        {
            return await _context.AdminAuditLogs
                .Include(log => log.Admin)
                .ToListAsync();
        }

        public async Task<AdminAuditLog> GetAdminAuditLogByIdAsync(int id)
        {
            return await _context.AdminAuditLogs
                .Include(l => l.Admin)
                .FirstOrDefaultAsync(l => l.LogId == id);
        }

        public async Task<AdminAuditLog> CreateAdminAuditLogAsync(AdminAuditLog log)
        {
            _context.AdminAuditLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<bool> UpdateAdminAuditLogAsync(int id, AdminAuditLog log)
        {
            if (id != log.LogId)
                return false;

            _context.Entry(log).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdminAuditLogExistsAsync(id))
                    return false;
                throw;
            }
        }
    }
}

