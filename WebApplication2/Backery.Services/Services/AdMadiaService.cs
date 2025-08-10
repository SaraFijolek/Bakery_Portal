using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;

public class AdMadiaService : IAdMadiaService
{
    private readonly AppDbContext _context;

    public AdMadiaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResultService<IEnumerable<AdMediaResponseDto>>> GetAllAdMediaAsync()
    {
        try
        {
            var adMediaEntities = await _context.Admedias
                .Include(m => m.Ad)
                .ToListAsync();

            // Manualne mapowanie zamiast extension method
            var result = adMediaEntities.Select(entity => new AdMediaResponseDto
            {
                MediaId = entity.MediaId,
                AdId = entity.AdId,
                MediaType = entity.MediaType,
            }).ToList();

            return ResultService<IEnumerable<AdMediaResponseDto>>.GoodResult(
                "AdMedia retrieved successfully",
                200,
                result);
        }
        catch (Exception ex)
        {
            return ResultService<IEnumerable<AdMediaResponseDto>>.BadResult(
                "Error retrieving AdMedia",
                500,
                new List<string> { ex.Message });
        }
    }

    public async Task<ResultService<AdMediaResponseDto>> GetAdMediaByIdAsync(int id)
    {
        try
        {
            var adMediaEntity = await _context.Admedias
                .Include(m => m.Ad)
                .FirstOrDefaultAsync(m => m.MediaId == id);

            if (adMediaEntity == null)
            {
                return ResultService<AdMediaResponseDto>.BadResult(
                    "AdMedia not found",
                    404,
                    new List<string> { $"AdMedia with ID {id} does not exist" });
            }

            // Manualne mapowanie
            var result = new AdMediaResponseDto
            {
                MediaId = adMediaEntity.MediaId,
                AdId = adMediaEntity.AdId,
                MediaType = adMediaEntity.MediaType,
            };

            return ResultService<AdMediaResponseDto>.GoodResult(
                "AdMedia retrieved successfully",
                200,
                result);
        }
        catch (Exception ex)
        {
            return ResultService<AdMediaResponseDto>.BadResult(
                "Error retrieving AdMedia",
                500,
                new List<string> { ex.Message });
        }
    }

    public async Task<ResultService<AdMediaResponseDto>> CreateAdMediaAsync(CreateAdMediaDto createAdMediaDto)
    {
        try
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
            var result = new AdMediaResponseDto
            {
                MediaId = createdAdMedia!.MediaId,
                AdId = createdAdMedia.AdId,
                MediaType = createdAdMedia.MediaType,
            };

            return ResultService<AdMediaResponseDto>.GoodResult(
                "AdMedia created successfully",
                201,
                result);
        }
        catch (Exception ex)
        {
            return ResultService<AdMediaResponseDto>.BadResult(
                "Error creating AdMedia",
                500,
                new List<string> { ex.Message });
        }
    }

    public async Task<ResultService<bool>> UpdateAdMediaAsync(int id, UpdateAdMediaDto updateAdMediaDto)
    {
        try
        {
            if (id != updateAdMediaDto.MediaId)
            {
                return ResultService<bool>.BadResult(
                    "ID mismatch",
                    400,
                    new List<string> { "The ID in the URL does not match the ID in the request body" });
            }

            var existingAdMedia = await _context.Admedias.FindAsync(id);
            if (existingAdMedia == null)
            {
                return ResultService<bool>.BadResult(
                    "AdMedia not found",
                    404,
                    new List<string> { $"AdMedia with ID {id} does not exist" });
            }

            // Manualne mapowanie z DTO na Entity
            existingAdMedia.AdId = updateAdMediaDto.AdId;
            existingAdMedia.MediaType = updateAdMediaDto.MediaType;
            // Dodaj inne właściwości zgodnie z twoją strukturą

            _context.Entry(existingAdMedia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ResultService<bool>.GoodResult(
                    "AdMedia updated successfully",
                    200,
                    true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdMediaExistsAsync(id))
                {
                    return ResultService<bool>.BadResult(
                        "AdMedia not found",
                        404,
                        new List<string> { $"AdMedia with ID {id} no longer exists" });
                }
                throw;
            }
        }
        catch (Exception ex)
        {
            return ResultService<bool>.BadResult(
                "Error updating AdMedia",
                500,
                new List<string> { ex.Message });
        }
    }

    public async Task<ResultService<bool>> DeleteAdMediaAsync(int id)
    {
        try
        {
            var adMedia = await _context.Admedias.FindAsync(id);
            if (adMedia == null)
            {
                return ResultService<bool>.BadResult(
                    "AdMedia not found",
                    404,
                    new List<string> { $"AdMedia with ID {id} does not exist" });
            }

            _context.Admedias.Remove(adMedia);
            await _context.SaveChangesAsync();

            return ResultService<bool>.GoodResult(
                "AdMedia deleted successfully",
                200,
                true);
        }
        catch (Exception ex)
        {
            return ResultService<bool>.BadResult(
                "Error deleting AdMedia",
                500,
                new List<string> { ex.Message });
        }
    }

    public async Task<bool> AdMediaExistsAsync(int id)
    {
        return await _context.Admedias.AnyAsync(e => e.MediaId == id);
    }
}