using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.DTOs;
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

        public async Task<List<AdminSessionReadDto>> GetAllAdminSessionsAsync()
        {
            var sessions = await _context.AdminSessions
                .Include(s => s.Admin)
                .ToListAsync();

            return AdminSessionMapper.ToDtoList(sessions);
        }

        public async Task<List<AdminSessionListDto>> GetAllAdminSessionsListAsync()
        {
            var sessions = await _context.AdminSessions
                .Include(s => s.Admin)
                .ToListAsync();

            return AdminSessionMapper.ToListDtoList(sessions);
        }

        public async Task<AdminSessionReadDto> GetAdminSessionByIdAsync(int id)
        {
            var session = await _context.AdminSessions
                .Include(s => s.Admin)
                .FirstOrDefaultAsync(s => s.SessionId == id);

            return session != null ? AdminSessionMapper.ToDto(session) : null;
        }

        public async Task<AdminSessionReadDto> CreateAdminSessionAsync(CreateAdminSessionDto sessionDto)
        {
            var session = AdminSessionMapper.ToModel(sessionDto);

            _context.AdminSessions.Add(session);
            await _context.SaveChangesAsync();

            // Reload with Admin data
            var createdSession = await _context.AdminSessions
                .Include(s => s.Admin)
                .FirstOrDefaultAsync(s => s.SessionId == session.SessionId);

            return AdminSessionMapper.ToDto(createdSession);
        }

        public async Task<bool> UpdateAdminSessionAsync(int id, UpdateAdminSessionDto sessionDto)
        {
            var existingSession = await _context.AdminSessions.FindAsync(id);
            if (existingSession == null)
            {
                return false;
            }

            AdminSessionMapper.UpdateModel(existingSession, sessionDto);

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
