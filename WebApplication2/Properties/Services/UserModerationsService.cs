using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class UserModerationsService : IUserModerationsService
    {
        private readonly AppDbContext _context;

        public UserModerationsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserModeration>> GetUserModerationsAsync()
        {
            return await _context.UserModerations
                .Include(um => um.User)
                .Include(um => um.Admin)
                .ToListAsync();
        }

        public async Task<UserModeration?> GetUserModerationByIdAsync(int id)
        {
            return await _context.UserModerations
                .Include(um => um.User)
                .Include(um => um.Admin)
                .FirstOrDefaultAsync(um => um.ModerationId == id);
        }

        public async Task<UserModeration> CreateUserModerationAsync(UserModeration moderation)
        {
            _context.UserModerations.Add(moderation);
            await _context.SaveChangesAsync();
            return moderation;
        }

        public async Task<bool> UpdateUserModerationAsync(int id, UserModeration moderation)
        {
            if (id != moderation.ModerationId)
                return false;

            _context.Entry(moderation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserModerationExistsAsync(id))
                    return false;
                throw;
            }
        }

        public async Task<bool> DeleteUserModerationAsync(int id)
        {
            var moderation = await _context.UserModerations.FindAsync(id);
            if (moderation == null)
                return false;

            _context.UserModerations.Remove(moderation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserModerationExistsAsync(int id)
        {
            return await _context.UserModerations.AnyAsync(e => e.ModerationId == id);
        }
    }
}

