using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;


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

            // Manualne mapowanie zamiast extension method
            return adMediaEntities.Select(entity => new AdMediaResponseDto
            {
                MediaId = entity.MediaId,
                AdId = entity.AdId,
                MediaType = entity.MediaType,
            }
            ).ToList();
        }


        public async Task<AdMediaResponseDto?> GetAdMediaByIdAsync(int id)
        {
            var adMediaEntity = await _context.Admedias
                .Include(m => m.Ad)
                .FirstOrDefaultAsync(m => m.MediaId == id);

            if (adMediaEntity == null)
                return null;

            // Manualne mapowanie
            return new AdMediaResponseDto
            {
                MediaId = adMediaEntity.MediaId,
                AdId = adMediaEntity.AdId,
                MediaType = adMediaEntity.MediaType,
            };
        }
                
                // Dodaj inne właściwości zgodnie z twoją strukturą DTO
               
          

        public async Task<AdMediaResponseDto> CreateAdMediaAsync(CreateAdMediaDto createAdMediaDto)
        {
            // Manualne mapowanie z DTO na Entity
            var adMediaEntity = new Admedia
            {
                AdId = createAdMediaDto.AdId,
                MediaType = createAdMediaDto.MediaType,

                // Dodaj inne właściwości zgodnie z twoją strukturą
            };

            _context.Admedias.Add(adMediaEntity);
            await _context.SaveChangesAsync();

            // Pobierz utworzony obiekt z danymi Ad
            var createdAdMedia = await _context.Admedias
                .Include(m => m.Ad)
                .FirstOrDefaultAsync(m => m.MediaId == adMediaEntity.MediaId);

            // Manualne mapowanie z Entity na DTO
            return new AdMediaResponseDto
            {
                MediaId = createdAdMedia!.MediaId,
                AdId = createdAdMedia.AdId,
                MediaType = createdAdMedia.MediaType,
            };
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

            // Manualne mapowanie z DTO na Entity
            existingAdMedia.AdId = updateAdMediaDto.AdId;
            existingAdMedia.MediaType = updateAdMediaDto.MediaType;
           
            // Dodaj inne właściwości zgodnie z twoją strukturą

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
