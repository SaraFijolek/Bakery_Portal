using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    
    public class UserSocialAuthsService<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static UserSocialAuthsService<T> GoodResult(
            string message,
            int statusCode,
            T? data = default)
            => new UserSocialAuthsService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = true,
                Data = data
            };

        public static UserSocialAuthsService<T> BadResult(
            string message,
            int statusCode,
            List<string>? errors = null,
            T? data = default)
            => new UserSocialAuthsService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = false,
                Errors = errors ?? new List<string>(),
                Data = data
            };
    }

    public class UserSocialAuthService : IUserSocialAuthService
    {
        private readonly AppDbContext _context;

        public UserSocialAuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserSocialAuthsService<List<UserSocialAuthDto>>> GetUserSocialAuthsAsync()
        {
            try
            {
                var userSocialAuths = await _context.UserSocialAuths
                    .Include(ua => ua.User)
                    .ToListAsync();

                var userSocialAuthDtos = userSocialAuths.Select(MapToUserSocialAuthDto).ToList();

                return UserSocialAuthsService<List<UserSocialAuthDto>>.GoodResult(
                    "User social authentications retrieved successfully",
                    200,
                    userSocialAuthDtos);
            }
            catch (Exception ex)
            {
                return UserSocialAuthsService<List<UserSocialAuthDto>>.BadResult(
                    "Failed to retrieve user social authentications",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<UserSocialAuthsService<UserSocialAuthDto>> GetUserSocialAuthByIdAsync(int id)
        {
            try
            {
                var userSocialAuth = await _context.UserSocialAuths
                    .Include(ua => ua.User)
                    .FirstOrDefaultAsync(ua => ua.SocialAuthId == id);

                if (userSocialAuth == null)
                {
                    return UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                        $"User social authentication with ID {id} not found",
                        404);
                }

                var userSocialAuthDto = MapToUserSocialAuthDto(userSocialAuth);

                return UserSocialAuthsService<UserSocialAuthDto>.GoodResult(
                    "User social authentication retrieved successfully",
                    200,
                    userSocialAuthDto);
            }
            catch (Exception ex)
            {
                return UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                    "Failed to retrieve user social authentication",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<UserSocialAuthsService<UserSocialAuthDto>> CreateUserSocialAuthAsync(CreateUserSocialAuthDto createDto)
        {
            try
            {
                // Walidacja - sprawdź czy użytkownik istnieje
                var userExists = await _context.Users.AnyAsync(u => u.Id == createDto.UserId);
                if (!userExists)
                {
                    return UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                        "Validation failed",
                        400,
                        new List<string> { $"User with ID {createDto.UserId} does not exist" });
                }

                // Sprawdź czy już istnieje połączenie dla tego użytkownika i providera
                var existingAuth = await _context.UserSocialAuths
                    .AnyAsync(ua => ua.UserId == createDto.UserId && ua.Provider == createDto.Provider);
                if (existingAuth)
                {
                    return UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                        "Validation failed",
                        400,
                        new List<string> { $"User already has authentication for provider '{createDto.Provider}'" });
                }

                // Sprawdź czy ProviderId już istnieje dla tego providera
                var providerIdExists = await _context.UserSocialAuths
                    .AnyAsync(ua => ua.Provider == createDto.Provider && ua.ProviderId == createDto.ProviderId);
                if (providerIdExists)
                {
                    return UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                        "Validation failed",
                        400,
                        new List<string> { $"Provider ID '{createDto.ProviderId}' already exists for provider '{createDto.Provider}'" });
                }

                var userSocialAuth = new UserSocialAuth
                {
                    UserId = createDto.UserId,
                    Provider = createDto.Provider?.Trim(),
                    ProviderId = createDto.ProviderId?.Trim(),
                    AccessToken = createDto.AccessToken,
                    RefreshToken = createDto.RefreshToken,
                    TokenExpires = createDto.TokenExpires,
                    ProfileData = createDto.ProfileData,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.UserSocialAuths.Add(userSocialAuth);
                await _context.SaveChangesAsync();

                // Załaduj nawigację bez dodatkowego zapytania
                await _context.Entry(userSocialAuth)
                    .Reference(ua => ua.User)
                    .LoadAsync();

                var userSocialAuthDto = MapToUserSocialAuthDto(userSocialAuth);

                return UserSocialAuthsService<UserSocialAuthDto>.GoodResult(
                    "User social authentication created successfully",
                    201,
                    userSocialAuthDto);
            }
            catch (Exception ex)
            {
                return UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                    "Failed to create user social authentication",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<UserSocialAuthsService<UserSocialAuthDto>> UpdateUserSocialAuthAsync(int id, UpdateUserSocialAuthDto updateDto)
        {
            try
            {
                var existingAuth = await _context.UserSocialAuths.FindAsync(id);
                if (existingAuth == null)
                {
                    return UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                        $"User social authentication with ID {id} not found",
                        404);
                }

                // Walidacja - sprawdź czy użytkownik istnieje
                var userExists = await _context.Users.AnyAsync(u => u.Id == updateDto.UserId);
                if (!userExists)
                {
                    return UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                        "Validation failed",
                        400,
                        new List<string> { $"User with ID {updateDto.UserId} does not exist" });
                }

                // Sprawdź czy już istnieje połączenie dla tego użytkownika i providera (pomijając aktualny rekord)
                var existingAuthForUser = await _context.UserSocialAuths
                    .AnyAsync(ua => ua.UserId == updateDto.UserId &&
                                   ua.Provider == updateDto.Provider &&
                                   ua.SocialAuthId != id);
                if (existingAuthForUser)
                {
                    return UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                        "Validation failed",
                        400,
                        new List<string> { $"User already has authentication for provider '{updateDto.Provider}'" });
                }

                // Sprawdź czy ProviderId już istnieje dla tego providera (pomijając aktualny rekord)
                var providerIdExists = await _context.UserSocialAuths
                    .AnyAsync(ua => ua.Provider == updateDto.Provider &&
                                   ua.ProviderId == updateDto.ProviderId &&
                                   ua.SocialAuthId != id);
                if (providerIdExists)
                {
                    return UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                        "Validation failed",
                        400,
                        new List<string> { $"Provider ID '{updateDto.ProviderId}' already exists for provider '{updateDto.Provider}'" });
                }

                // Aktualizacja właściwości
                existingAuth.UserId = updateDto.UserId;
                existingAuth.Provider = updateDto.Provider?.Trim();
                existingAuth.ProviderId = updateDto.ProviderId?.Trim();
                existingAuth.AccessToken = updateDto.AccessToken;
                existingAuth.RefreshToken = updateDto.RefreshToken;
                existingAuth.TokenExpires = updateDto.TokenExpires;
                existingAuth.ProfileData = updateDto.ProfileData;
                existingAuth.UpdatedAt = DateTime.UtcNow;

                _context.Entry(existingAuth).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Załaduj nawigację
                await _context.Entry(existingAuth)
                    .Reference(ua => ua.User)
                    .LoadAsync();

                var userSocialAuthDto = MapToUserSocialAuthDto(existingAuth);

                return UserSocialAuthsService<UserSocialAuthDto>.GoodResult(
                    "User social authentication updated successfully",
                    200,
                    userSocialAuthDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserSocialAuthExistsAsync(id))
                {
                    return UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                        $"User social authentication with ID {id} not found",
                        404);
                }

                return UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                    "Concurrency conflict occurred while updating user social authentication",
                    409);
            }
            catch (Exception ex)
            {
                return UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                    "Failed to update user social authentication",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<UserSocialAuthsService<bool>> DeleteUserSocialAuthAsync(int id)
        {
            try
            {
                var auth = await _context.UserSocialAuths.FindAsync(id);
                if (auth == null)
                {
                    return UserSocialAuthsService<bool>.BadResult(
                        $"User social authentication with ID {id} not found",
                        404,
                        data: false);
                }

                _context.UserSocialAuths.Remove(auth);
                await _context.SaveChangesAsync();

                return UserSocialAuthsService<bool>.GoodResult(
                    "User social authentication deleted successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return UserSocialAuthsService<bool>.BadResult(
                    "Failed to delete user social authentication",
                    500,
                    new List<string> { ex.Message },
                    false);
            }
        }

        public async Task<bool> UserSocialAuthExistsAsync(int id)
        {
            return await _context.UserSocialAuths.AnyAsync(e => e.SocialAuthId == id);
        }

       
        public async Task<UserSocialAuthsService<UserSocialAuthDto>> GetUserSocialAuthByProviderAsync(string userId, string provider)
        {
            try
            {
                var userSocialAuth = await _context.UserSocialAuths
                    .Include(ua => ua.User)
                    .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.Provider == provider);

                if (userSocialAuth == null)
                {
                    return UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                        $"User social authentication for user {userId} and provider '{provider}' not found",
                        404);
                }

                var userSocialAuthDto = MapToUserSocialAuthDto(userSocialAuth);

                return UserSocialAuthsService<UserSocialAuthDto>.GoodResult(
                    "User social authentication retrieved successfully",
                    200,
                    userSocialAuthDto);
            }
            catch (Exception ex)
            {
                return UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                    "Failed to retrieve user social authentication by provider",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<UserSocialAuthsService<bool>> RefreshTokenAsync(int id, string newAccessToken, string? newRefreshToken = null, DateTime? newTokenExpires = null)
        {
            try
            {
                var existingAuth = await _context.UserSocialAuths.FindAsync(id);
                if (existingAuth == null)
                {
                    return UserSocialAuthsService<bool>.BadResult(
                        $"User social authentication with ID {id} not found",
                        404,
                        data: false);
                }

                existingAuth.AccessToken = newAccessToken;
                if (newRefreshToken != null)
                    existingAuth.RefreshToken = newRefreshToken;
                if (newTokenExpires.HasValue)
                    existingAuth.TokenExpires = newTokenExpires.Value;
                existingAuth.UpdatedAt = DateTime.UtcNow;

                _context.Entry(existingAuth).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return UserSocialAuthsService<bool>.GoodResult(
                    "Token refreshed successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return UserSocialAuthsService<bool>.BadResult(
                    "Failed to refresh token",
                    500,
                    new List<string> { ex.Message },
                    false);
            }
        }

      
        private static UserSocialAuthDto MapToUserSocialAuthDto(UserSocialAuth userSocialAuth)
        {
            return new UserSocialAuthDto
            {
                SocialAuthId = userSocialAuth.SocialAuthId,
                UserId = userSocialAuth.UserId,
                Provider = userSocialAuth.Provider,
                ProviderId = userSocialAuth.ProviderId,
                AccessToken = userSocialAuth.AccessToken,
                RefreshToken = userSocialAuth.RefreshToken,
                TokenExpires = userSocialAuth.TokenExpires,
                ProfileData = userSocialAuth.ProfileData,
                CreatedAt = userSocialAuth.CreatedAt,
                UpdatedAt = userSocialAuth.UpdatedAt
            };
        }
    }
}