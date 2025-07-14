using Microsoft.EntityFrameworkCore;
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

        public async Task<List<AdminAuditLog>> GetAdminAuditLogsAsync()
        {
            return await _context.AdminAuditLogs
                .Include(log => log.Admin)
                .ToListAsync();
        }

        public async Task<AdminAuditLog?> GetAdminAuditLogByIdAsync(int id)
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
    }
}

