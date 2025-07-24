using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
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

        public async Task<IEnumerable<AdModerationDto>> GetAdModerationsAsync()
        {
            var adModerations = await _context.AdModerations
                .Include(m => m.Ad)
                .Include(m => m.Admin)
                .ToListAsync();

            return adModerations.Select(m => new AdModerationDto
            {
                ModerationId = m.ModerationId,
                AdId = m.AdId,
                Action = m.Action,
                Reason = m.Reason,
                CreatedAt = m.CreatedAt,
                
            });
        }

        public async Task<AdModerationDto?> GetAdModerationByIdAsync(int id)
        {
            var adModeration = await _context.AdModerations
                .Include(m => m.Ad)
                .Include(m => m.Admin)
                .FirstOrDefaultAsync(m => m.ModerationId == id);

            if (adModeration == null)
                return null;

            return new AdModerationDto
            {
                ModerationId = adModeration.ModerationId,
                AdId = adModeration.AdId,
                Action = adModeration.Action,
                Reason = adModeration.Reason,
                CreatedAt = adModeration.CreatedAt,
                
               
            };
        }

        public async Task<AdModerationDto> CreateAdModerationAsync(CreateAdModerationDto createDto)
        {
            var adModeration = new AdModeration
            {
                AdId = createDto.AdId,
                AdminId = createDto.AdminId,
                Action = createDto.Action,
                Reason = createDto.Reason,
                CreatedAt = DateTime.Now
            };

            _context.AdModerations.Add(adModeration);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            var createdModeration = await _context.AdModerations
                .Include(m => m.Ad)
                .Include(m => m.Admin)
                .FirstOrDefaultAsync(m => m.ModerationId == adModeration.ModerationId);

            return new AdModerationDto
            {
                ModerationId = createdModeration.ModerationId,
                AdId = createdModeration.AdId,
                Action = createdModeration.Action,
                Reason = createdModeration.Reason,
                CreatedAt = createdModeration.CreatedAt,
               
            };
        }

        public async Task<AdModerationDto> UpdateAdModerationAsync(int id, UpdateAdModerationDto updateDto)
        {
            var existingModeration = await _context.AdModerations.FindAsync(id);
            if (existingModeration == null)
                throw new KeyNotFoundException($"Ad moderation with ID {id} not found");

            existingModeration.AdId = updateDto.AdId;
            existingModeration.AdminId = updateDto.AdminId;
            existingModeration.Action = updateDto.Action;
            existingModeration.Reason = updateDto.Reason;

            _context.Entry(existingModeration).State = EntityState.Modified;

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

            // Reload with navigation properties
            var updatedModeration = await _context.AdModerations
                .Include(m => m.Ad)
                .Include(m => m.Admin)
                .FirstOrDefaultAsync(m => m.ModerationId == id);

            return new AdModerationDto
            {
                ModerationId = updatedModeration.ModerationId,
                AdId = updatedModeration.AdId,
                Action = updatedModeration.Action,
                Reason = updatedModeration.Reason,
                CreatedAt = updatedModeration.CreatedAt,
              
            };
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

