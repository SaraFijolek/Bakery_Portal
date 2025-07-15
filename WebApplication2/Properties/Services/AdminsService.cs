using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class AdminsService : IAdminsService
    {
        private readonly AppDbContext _context;

        public AdminsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Admin>> GetAllAdminsAsync()
        {
            return await _context.Admins
                .Include(a => a.AdminSessions)
                .Include(a => a.AdminAuditLogs)
                .Include(a => a.UserModerations)
                .Include(a => a.AdModerations)
                .Include(a => a.ReportedContents)
                .ToListAsync();
        }

        public async Task<Admin?> GetAdminByIdAsync(int id)
        {
            return await _context.Admins
                .Include(a => a.AdminSessions)
                .Include(a => a.AdminAuditLogs)
                .Include(a => a.UserModerations)
                .Include(a => a.AdModerations)
                .Include(a => a.ReportedContents)
                .FirstOrDefaultAsync(a => a.AdminId == id);
        }

        public async Task<Admin> CreateAdminAsync(Admin admin)
        {
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            return admin;
        }

        public async Task<bool> UpdateAdminAsync(int id, Admin admin)
        {
            if (id != admin.AdminId)
            {
                return false;
            }

            _context.Entry(admin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdminExistsAsync(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAdminAsync(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return false;
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdminExistsAsync(int id)
        {
            return await _context.Admins.AnyAsync(e => e.AdminId == id);
        }
    }
}
