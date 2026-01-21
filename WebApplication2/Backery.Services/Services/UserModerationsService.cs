using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class UserModerationService<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static UserModerationService<T> GoodResult(
            string message,
            int statusCode,
            T? data = default) => new UserModerationService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = true,
                Data = data
            };

        public static UserModerationService<T> BadResult(
            string message,
            int statusCode,
            List<string>? errors = null,
            T? data = default) => new UserModerationService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = false,
                Errors = errors ?? new List<string>(),
                Data = data
            };
    }



    public class UserModerationsService : IUserModerationsService
    {
        private readonly AppDbContext _context;

        public UserModerationsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserModerationService<List<UserModerationDto>>> GetUserModerationsAsync()
        {
            try
            {
                var moderations = await _context.UserModerations
                    .Include(um => um.User)
                    .Include(um => um.Admin)
                    .ToListAsync();

                var result = moderations.Select(MapToDto).ToList();

                return UserModerationService<List<UserModerationDto>>.GoodResult(
                    "User moderations retrieved successfully",
                    200,
                    result);
            }
            catch (Exception ex)
            {
                return UserModerationService<List<UserModerationDto>>.BadResult(
                    "Error retrieving user moderations",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<UserModerationService<List<UserModerationListDto>>> GetUserModerationsListAsync()
        {
            try
            {
                var moderations = await _context.UserModerations
                    .Include(um => um.User)
                    .Include(um => um.Admin)
                    .ToListAsync();

                var result = moderations.Select(MapToListDto).ToList();

                return UserModerationService<List<UserModerationListDto>>.GoodResult(
                    "User moderations list retrieved successfully",
                    200,
                    result);
            }
            catch (Exception ex)
            {
                return UserModerationService<List<UserModerationListDto>>.BadResult(
                    "Error retrieving user moderations list",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<UserModerationService<UserModerationDto>> GetUserModerationByIdAsync(int id)
        {
            try
            {
                var moderation = await _context.UserModerations
                    .Include(um => um.User)
                    .Include(um => um.Admin)
                    .FirstOrDefaultAsync(um => um.ModerationId == id);

                if (moderation == null)
                {
                    return UserModerationService<UserModerationDto>.BadResult(
                        "User moderation not found",
                        404);
                }

                return UserModerationService<UserModerationDto>.GoodResult(
                    "User moderation retrieved successfully",
                    200,
                    MapToDto(moderation));
            }
            catch (Exception ex)
            {
                return UserModerationService<UserModerationDto>.BadResult(
                    "Error retrieving user moderation",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<UserModerationService<List<UserModerationDto>>> GetUserModerationsByUserIdAsync(string userId)
        {
            try
            {
                var moderations = await _context.UserModerations
                    .Include(um => um.User)
                    .Include(um => um.Admin)
                    .Where(um => um.UserId == userId)
                    .ToListAsync();

                var result = moderations.Select(MapToDto).ToList();

                return UserModerationService<List<UserModerationDto>>.GoodResult(
                    "User moderations retrieved successfully",
                    200,
                    result);
            }
            catch (Exception ex)
            {
                return UserModerationService<List<UserModerationDto>>.BadResult(
                    "Error retrieving user moderations",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<UserModerationService<List<UserModerationDto>>> GetActiveUserModerationsAsync()
        {
            try
            {
                var moderations = await _context.UserModerations
                    .Include(um => um.User)
                    .Include(um => um.Admin)
                    .Where(um => um.IsActive)
                    .ToListAsync();

                var result = moderations.Select(MapToDto).ToList();

                return UserModerationService<List<UserModerationDto>>.GoodResult(
                    "Active user moderations retrieved successfully",
                    200,
                    result);
            }
            catch (Exception ex)
            {
                return UserModerationService<List<UserModerationDto>>.BadResult(
                    "Error retrieving active user moderations",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<UserModerationService<UserModerationDto>> CreateUserModerationAsync(CreateUserModerationDto createDto)
        {
            try
            {
                var moderation = new UserModeration
                {
                    UserId = createDto.UserId,
                    AdminId = createDto.AdminId,
                    Action = createDto.Action,
                    Reason = createDto.Reason,
                    DurationHours = createDto.DurationHours,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = createDto.ExpiresAt,
                    IsActive = createDto.IsActive
                };

                _context.UserModerations.Add(moderation);
                await _context.SaveChangesAsync();

                // Reload with navigation properties
                var createdModeration = await _context.UserModerations
                    .Include(um => um.User)
                    .Include(um => um.Admin)
                    .FirstAsync(um => um.ModerationId == moderation.ModerationId);

                return UserModerationService<UserModerationDto>.GoodResult(
                    "User moderation created successfully",
                    201,
                    MapToDto(createdModeration));
            }
            catch (Exception ex)
            {
                return UserModerationService<UserModerationDto>.BadResult(
                    "Error creating user moderation",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<UserModerationService<bool>> UpdateUserModerationAsync(int id, UpdateUserModerationDto updateDto)
        {
            try
            {
                var moderation = await _context.UserModerations.FindAsync(id);
                if (moderation == null)
                {
                    return UserModerationService<bool>.BadResult(
                        "User moderation not found",
                        404);
                }

                moderation.UserId = updateDto.UserId;
                moderation.AdminId = updateDto.AdminId;
                moderation.Action = updateDto.Action;
                moderation.Reason = updateDto.Reason;
                moderation.DurationHours = updateDto.DurationHours;
                moderation.ExpiresAt = updateDto.ExpiresAt;
                moderation.IsActive = updateDto.IsActive;

                await _context.SaveChangesAsync();

                return UserModerationService<bool>.GoodResult(
                    "User moderation updated successfully",
                    200,
                    true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserModerationExistsAsync(id))
                {
                    return UserModerationService<bool>.BadResult(
                        "User moderation not found",
                        404);
                }

                return UserModerationService<bool>.BadResult(
                    "Concurrency error occurred while updating user moderation",
                    409,
                    new List<string> { "The record was modified by another user. Please refresh and try again." });
            }
            catch (Exception ex)
            {
                return UserModerationService<bool>.BadResult(
                    "Error updating user moderation",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<UserModerationService<bool>> DeleteUserModerationAsync(int id)
        {
            try
            {
                var moderation = await _context.UserModerations.FindAsync(id);
                if (moderation == null)
                {
                    return UserModerationService<bool>.BadResult(
                        "User moderation not found",
                        404);
                }

                _context.UserModerations.Remove(moderation);
                await _context.SaveChangesAsync();

                return UserModerationService<bool>.GoodResult(
                    "User moderation deleted successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return UserModerationService<bool>.BadResult(
                    "Error deleting user moderation",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<bool> UserModerationExistsAsync(int id)
        {
            return await _context.UserModerations.AnyAsync(e => e.ModerationId == id);
        }

        // Mapping methods
        private UserModerationDto MapToDto(UserModeration moderation)
        {
            return new UserModerationDto
            {
                ModerationId = moderation.ModerationId,
                UserId = moderation.UserId,
                AdminId = moderation.AdminId,
                Action = moderation.Action,
                Reason = moderation.Reason,
                DurationHours = moderation.DurationHours,
                CreatedAt = moderation.CreatedAt,
                ExpiresAt = moderation.ExpiresAt,
                IsActive = moderation.IsActive,
                User = moderation.User != null ? new UserDto
                {
                    UserId = moderation.UserId,
                    Username = moderation.User.UserName,
                    Email = moderation.User.Email
                    // Add other properties as needed
                } : null,
                Admin = moderation.Admin != null ? new AdminDto
                {
                    AdminId = moderation.Admin.AdminId,
                    Username = moderation.Admin.Name,
                    Email = moderation.Admin.Email
                    // Add other properties as needed
                } : null
            };
        }

        private UserModerationListDto MapToListDto(UserModeration moderation)
        {
            return new UserModerationListDto
            {
                ModerationId = moderation.ModerationId,
                UserId = moderation.UserId,
                AdminId = moderation.AdminId,
                Action = moderation.Action,
                Reason = moderation.Reason,
                DurationHours = moderation.DurationHours,
                CreatedAt = moderation.CreatedAt,
                ExpiresAt = moderation.ExpiresAt,
                IsActive = moderation.IsActive,
                UserName = moderation.User?.UserName ?? "Unknown",
                AdminName = moderation.Admin?.Name ?? "Unknown"
            };
        }
    }
}