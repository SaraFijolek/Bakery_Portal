using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class AdModerationService<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static AdModerationService<T> GoodResult(
            string message,
            int statusCode,
            T? data = default)
            => new AdModerationService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = true,
                Data = data
            };

        public static AdModerationService<T> BadResult(
            string message,
            int statusCode,
            List<string>? errors = null,
            T? data = default)
            =>
            new AdModerationService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = false,
                Errors = errors ?? new List<string>(),
                Data = data
            };
    }



    public class AdModerationsService : IAdModerationsService
    {
        private readonly AppDbContext _context;

        public AdModerationsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdModerationService<IEnumerable<AdModerationDto>>> GetAdModerationsAsync()
        {
            try
            {
                var adModerations = await _context.AdModerations
                    .Include(m => m.Ad)
                    .Include(m => m.Admin)
                    .ToListAsync();

                var moderationDtos = adModerations.Select(m => new AdModerationDto
                {
                    ModerationId = m.ModerationId,
                    AdId = m.AdId,
                    Action = m.Action,
                    Reason = m.Reason,
                    CreatedAt = m.CreatedAt,
                });

                return AdModerationService<IEnumerable<AdModerationDto>>.GoodResult(
                    "Ad moderations retrieved successfully",
                    200,
                    moderationDtos);
            }
            catch (Exception ex)
            {
                return AdModerationService<IEnumerable<AdModerationDto>>.BadResult(
                    "Error occurred while retrieving ad moderations",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdModerationService<AdModerationDto>> GetAdModerationByIdAsync(int id)
        {
            try
            {
                var adModeration = await _context.AdModerations
                    .Include(m => m.Ad)
                    .Include(m => m.Admin)
                    .FirstOrDefaultAsync(m => m.ModerationId == id);

                if (adModeration == null)
                {
                    return AdModerationService<AdModerationDto>.BadResult(
                        $"Ad moderation with ID {id} not found",
                        404);
                }

                var moderationDto = new AdModerationDto
                {
                    ModerationId = adModeration.ModerationId,
                    AdId = adModeration.AdId,
                    Action = adModeration.Action,
                    Reason = adModeration.Reason,
                    CreatedAt = adModeration.CreatedAt,
                };

                return AdModerationService<AdModerationDto>.GoodResult(
                    "Ad moderation retrieved successfully",
                    200,
                    moderationDto);
            }
            catch (Exception ex)
            {
                return AdModerationService<AdModerationDto>.BadResult(
                    "Error occurred while retrieving ad moderation",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdModerationService<AdModerationDto>> CreateAdModerationAsync(CreateAdModerationDto createDto)
        {
            try
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

                var moderationDto = new AdModerationDto
                {
                    ModerationId = createdModeration.ModerationId,
                    AdId = createdModeration.AdId,
                    Action = createdModeration.Action,
                    Reason = createdModeration.Reason,
                    CreatedAt = createdModeration.CreatedAt,
                };

                return AdModerationService<AdModerationDto>.GoodResult(
                    "Ad moderation created successfully",
                    201,
                    moderationDto);
            }
            catch (Exception ex)
            {
                return AdModerationService<AdModerationDto>.BadResult(
                    "Error occurred while creating ad moderation",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdModerationService<AdModerationDto>> UpdateAdModerationAsync(int id, UpdateAdModerationDto updateDto)
        {
            try
            {
                var existingModeration = await _context.AdModerations.FindAsync(id);
                if (existingModeration == null)
                {
                    return AdModerationService<AdModerationDto>.BadResult(
                        $"Ad moderation with ID {id} not found",
                        404);
                }

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
                    {
                        return AdModerationService<AdModerationDto>.BadResult(
                            $"Ad moderation with ID {id} not found",
                            404);
                    }
                    throw;
                }

                // Reload with navigation properties
                var updatedModeration = await _context.AdModerations
                    .Include(m => m.Ad)
                    .Include(m => m.Admin)
                    .FirstOrDefaultAsync(m => m.ModerationId == id);

                var moderationDto = new AdModerationDto
                {
                    ModerationId = updatedModeration.ModerationId,
                    AdId = updatedModeration.AdId,
                    Action = updatedModeration.Action,
                    Reason = updatedModeration.Reason,
                    CreatedAt = updatedModeration.CreatedAt,
                };

                return AdModerationService<AdModerationDto>.GoodResult(
                    "Ad moderation updated successfully",
                    200,
                    moderationDto);
            }
            catch (Exception ex)
            {
                return AdModerationService<AdModerationDto>.BadResult(
                    "Error occurred while updating ad moderation",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdModerationService<bool>> DeleteAdModerationAsync(int id)
        {
            try
            {
                var moderation = await _context.AdModerations.FindAsync(id);
                if (moderation == null)
                {
                    return AdModerationService<bool>.BadResult(
                        $"Ad moderation with ID {id} not found",
                        404);
                }

                _context.AdModerations.Remove(moderation);
                await _context.SaveChangesAsync();

                return AdModerationService<bool>.GoodResult(
                    "Ad moderation deleted successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return AdModerationService<bool>.BadResult(
                    "Error occurred while deleting ad moderation",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<bool> AdModerationExistsAsync(int id)
        {
            return await _context.AdModerations.AnyAsync(e => e.ModerationId == id);
        }
    }
}