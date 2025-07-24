// Properties/Services/AdMadiaService.cs
using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Helpers;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class AdMadiaService : IAdMadiaService
    {
        private readonly AppDbContext _context;

        public AdMadiaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdMediaResponseDto>> GetAllAdMediaAsync()
        {
            var adMediaEntities = await _context.Admedias
                .Include(m => m.Ad)
                .ToListAsync();

            return adMediaEntities.ToAdMediaResponseDtos();
        }

        public async Task<AdMediaResponseDto?> GetAdMediaByIdAsync(int id)
        {
            var adMediaEntity = await _context.Admedias
                .Include(m => m.Ad)
                .FirstOrDefaultAsync(m => m.MediaId == id);

            return adMediaEntity?.ToAdMediaResponseDto();
        }

        public async Task<AdMediaResponseDto> CreateAdMediaAsync(CreateAdMediaDto createAdMediaDto)
        {
            var adMediaEntity = createAdMediaDto.ToEntity();

            _context.Admedias.Add(adMediaEntity);
            await _context.SaveChangesAsync();

            // Pobierz utworzony obiekt z danymi Ad
            var createdAdMedia = await _context.Admedias
                .Include(m => m.Ad)
                .FirstOrDefaultAsync(m => m.MediaId == adMediaEntity.MediaId);

            return createdAdMedia!.ToAdMediaResponseDto();
        }

        public async Task<bool> UpdateAdMediaAsync(int id, UpdateAdMediaDto updateAdMediaDto)
        {
            if (id != updateAdMediaDto.MediaId)
            {
                return false;
            }

            var existingAdMedia = await _context.Admedias.FindAsync(id);
            if (existingAdMedia == null)
            {
                return false;
            }

            updateAdMediaDto.MapToEntity(existingAdMedia);
            _context.Entry(existingAdMedia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdMediaExistsAsync(id))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> DeleteAdMediaAsync(int id)
        {
            var adMedia = await _context.Admedias.FindAsync(id);
            if (adMedia == null)
            {
                return false;
            }

            _context.Admedias.Remove(adMedia);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdMediaExistsAsync(int id)
        {
            return await _context.Admedias.AnyAsync(e => e.MediaId == id);
        }
    }
}