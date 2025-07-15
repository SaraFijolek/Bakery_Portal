using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class AdminsSessionsService : IAdminsSessionsService
    {
        private readonly AppDbContext _context;

        public AdminsSessionsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdminSession>> GetAllAdminSessionsAsync()
        {
            return await _context.AdminSessions
                .Include(s => s.Admin)
                .ToListAsync();
        }

        public async Task<AdminSession?> GetAdminSessionByIdAsync(int id)
        {
            return await _context.AdminSessions
                .Include(s => s.Admin)
                .FirstOrDefaultAsync(s => s.SessionId == id);
        }

        public async Task<AdminSession> CreateAdminSessionAsync(AdminSession session)
        {
            _context.AdminSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<bool> UpdateAdminSessionAsync(int id, AdminSession session)
        {
            if (id != session.SessionId)
            {
                return false;
            }

            _context.Entry(session).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdminSessionExistsAsync(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAdminSessionAsync(int id)
        {
            var session = await _context.AdminSessions.FindAsync(id);
            if (session == null)
            {
                return false;
            }

            _context.AdminSessions.Remove(session);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdminSessionExistsAsync(int id)
        {
            return await _context.AdminSessions.AnyAsync(e => e.SessionId == id);
        }
    }
}
