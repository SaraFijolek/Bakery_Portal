using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
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

        public async Task<List<UserModerationDto>> GetUserModerationsAsync()
        {
            var moderations = await _context.UserModerations
                .Include(um => um.User)
                .Include(um => um.Admin)
                .ToListAsync();

            return moderations.Select(MapToDto).ToList();
        }

        public async Task<List<UserModerationListDto>> GetUserModerationsListAsync()
        {
            var moderations = await _context.UserModerations
                .Include(um => um.User)
                .Include(um => um.Admin)
                .ToListAsync();

            return moderations.Select(MapToListDto).ToList();
        }

        public async Task<UserModerationDto?> GetUserModerationByIdAsync(int id)
        {
            var moderation = await _context.UserModerations
                .Include(um => um.User)
                .Include(um => um.Admin)
                .FirstOrDefaultAsync(um => um.ModerationId == id);

            return moderation != null ? MapToDto(moderation) : null;
        }

        public async Task<List<UserModerationDto>> GetUserModerationsByUserIdAsync(int userId)
        {
            var moderations = await _context.UserModerations
                .Include(um => um.User)
                .Include(um => um.Admin)
                .Where(um => um.UserId == userId)
                .ToListAsync();

            return moderations.Select(MapToDto).ToList();
        }

        public async Task<List<UserModerationDto>> GetActiveUserModerationsAsync()
        {
            var moderations = await _context.UserModerations
                .Include(um => um.User)
                .Include(um => um.Admin)
                .Where(um => um.IsActive)
                .ToListAsync();

            return moderations.Select(MapToDto).ToList();
        }

        public async Task<UserModerationDto> CreateUserModerationAsync(CreateUserModerationDto createDto)
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

            return MapToDto(createdModeration);
        }

        public async Task<bool> UpdateUserModerationAsync(int id, UpdateUserModerationDto updateDto)
        {
            var moderation = await _context.UserModerations.FindAsync(id);
            if (moderation == null)
                return false;

            moderation.UserId = updateDto.UserId;
            moderation.AdminId = updateDto.AdminId;
            moderation.Action = updateDto.Action;
            moderation.Reason = updateDto.Reason;
            moderation.DurationHours = updateDto.DurationHours;
            moderation.ExpiresAt = updateDto.ExpiresAt;
            moderation.IsActive = updateDto.IsActive;

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
                    UserId = moderation.User.UserId,
                    Username = moderation.User.Name,
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
                UserName = moderation.User?.Name ?? "Unknown",
                AdminName = moderation.Admin?.Name ?? "Unknown"
            };
        }
    }
}

