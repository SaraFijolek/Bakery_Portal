using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class AdModerationsService : IAdModerationsService
    {
        private readonly AppDbContext _context;

        public AdModerationsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdModeration>> GetAdModerationsAsync()
        {
            return await _context.AdModerations
                .Include(m => m.Ad)
                .Include(m => m.Admin)
                .ToListAsync();
        }

        public async Task<AdModeration?> GetAdModerationByIdAsync(int id)
        {
            return await _context.AdModerations
                .Include(m => m.Ad)
                .Include(m => m.Admin)
                .FirstOrDefaultAsync(m => m.ModerationId == id);
        }

        public async Task<AdModeration> CreateAdModerationAsync(AdModeration moderation)
        {
            _context.AdModerations.Add(moderation);
            await _context.SaveChangesAsync();
            return moderation;
        }

        public async Task<AdModeration> UpdateAdModerationAsync(int id, AdModeration moderation)
        {
            if (id != moderation.ModerationId)
                throw new ArgumentException("Moderation ID mismatch");

            _context.Entry(moderation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdModerationExistsAsync(id))
                    throw new KeyNotFoundException($"Ad moderation with ID {id} not found");
                throw;
            }

            return moderation;
        }

        public async Task<bool> DeleteAdModerationAsync(int id)
        {
            var moderation = await _context.AdModerations.FindAsync(id);
            if (moderation == null)
                return false;

            _context.AdModerations.Remove(moderation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdModerationExistsAsync(int id)
        {
            return await _context.AdModerations.AnyAsync(e => e.ModerationId == id);
        }
    }
}

