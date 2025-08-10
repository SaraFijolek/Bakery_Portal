using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;
using System.Net;

namespace WebApplication2.Properties.Services
{
    public class UsersService<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static UsersService<T> GoodResult(
            string message,
            int statusCode,
            T? data = default)
            => new UsersService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = true,
                Data = data
            };

        public static UsersService<T> BadResult(
            string message,
            int statusCode,
            List<string>? errors = null,
            T? data = default)
            =>
            new UsersService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = false,
                Errors = errors ?? new List<string>(),
                Data = data
            };
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UsersService<PagedResult<UserResponseDto>>> GetUsersAsync(UserQueryDto query)
        {
            try
            {
                var queryable = _context.Users.AsQueryable();

                if (!string.IsNullOrEmpty(query.SearchTerm))
                {
                    var searchTerm = query.SearchTerm.ToLower();
                    queryable = queryable.Where(u =>
                        u.Name.ToLower().Contains(searchTerm) ||
                        u.Email.ToLower().Contains(searchTerm));
                }

                queryable = query.SortBy?.ToLower() switch
                {
                    "name" => query.SortDescending ?
                        queryable.OrderByDescending(u => u.Name) :
                        queryable.OrderBy(u => u.Name),
                    "email" => query.SortDescending ?
                        queryable.OrderByDescending(u => u.Email) :
                        queryable.OrderBy(u => u.Email),
                    "createdat" => query.SortDescending ?
                        queryable.OrderByDescending(u => u.CreatedAt) :
                        queryable.OrderBy(u => u.CreatedAt),
                    _ => query.SortDescending ?
                        queryable.OrderByDescending(u => u.UserId) :
                        queryable.OrderBy(u => u.UserId)
                };

                var totalCount = await queryable.CountAsync();

                var users = await queryable
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .Select(u => new UserResponseDto
                    {
                        UserId = u.UserId,
                        Name = u.Name,
                        Email = u.Email,
                        CreatedAt = u.CreatedAt,
                        Phone = u.Phone
                    })
                    .ToListAsync();

                var pagedResult = new PagedResult<UserResponseDto>
                {
                    Items = users,
                    TotalCount = totalCount,
                    Page = query.Page,
                    PageSize = query.PageSize
                };

                return UsersService<PagedResult<UserResponseDto>>.GoodResult(
                    "Użytkownicy pobrani pomyślnie",
                    (int)HttpStatusCode.OK,
                    pagedResult
                );
            }
            catch (Exception ex)
            {
                return UsersService<PagedResult<UserResponseDto>>.BadResult(
                    "Błąd podczas pobierania użytkowników",
                    (int)HttpStatusCode.InternalServerError,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<UsersService<UserResponseDto?>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.UserId == id)
                    .Select(u => new UserResponseDto
                    {
                        UserId = u.UserId,
                        Name = u.Name,
                        Email = u.Email,
                        CreatedAt = u.CreatedAt,
                        Phone = u.Phone
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return UsersService<UserResponseDto?>.BadResult(
                        "Użytkownik nie został znaleziony",
                        (int)HttpStatusCode.NotFound
                    );
                }

                return UsersService<UserResponseDto?>.GoodResult(
                    "Użytkownik pobrany pomyślnie",
                    (int)HttpStatusCode.OK,
                    user
                );
            }
            catch (Exception ex)
            {
                return UsersService<UserResponseDto?>.BadResult(
                    "Błąd podczas pobierania użytkownika",
                    (int)HttpStatusCode.InternalServerError,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<UsersService<UserResponseDto>> CreateUserAsync(UserCreateDto userCreateDto)
        {
            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == userCreateDto.Email);

                if (existingUser != null)
                {
                    return UsersService<UserResponseDto>.BadResult(
                        "Użytkownik z tym adresem email już istnieje",
                        (int)HttpStatusCode.BadRequest,
                        new List<string> { "Email już istnieje w systemie" }
                    );
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(userCreateDto.Password);

                var user = new User
                {
                    Name = userCreateDto.Name,
                    Email = userCreateDto.Email,
                    PasswordHash = passwordHash,
                    Phone = userCreateDto.Phone,
                    CreatedAt = DateTime.UtcNow,
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userResponseDto = new UserResponseDto
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    Phone = user.Phone
                };

                return UsersService<UserResponseDto>.GoodResult(
                    "Użytkownik utworzony pomyślnie",
                    (int)HttpStatusCode.Created,
                    userResponseDto
                );
            }
            catch (Exception ex)
            {
                return UsersService<UserResponseDto>.BadResult(
                    "Błąd podczas tworzenia użytkownika",
                    (int)HttpStatusCode.InternalServerError,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<UsersService<bool>> UpdateUserAsync(int id, UserUpdateDto userUpdateDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return UsersService<bool>.BadResult(
                        "Użytkownik nie został znaleziony",
                        (int)HttpStatusCode.NotFound
                    );
                }

                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == userUpdateDto.Email && u.UserId != id);

                if (existingUser != null)
                {
                    return UsersService<bool>.BadResult(
                        "Adres email jest już używany przez innego użytkownika",
                        (int)HttpStatusCode.BadRequest,
                        new List<string> { "Email jest już zajęty" }
                    );
                }

                user.Name = userUpdateDto.Name;
                user.Email = userUpdateDto.Email;
                user.Phone = userUpdateDto.Phone;

                if (!string.IsNullOrEmpty(userUpdateDto.Password))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userUpdateDto.Password);
                }

                await _context.SaveChangesAsync();

                return UsersService<bool>.GoodResult(
                    "Użytkownik zaktualizowany pomyślnie",
                    (int)HttpStatusCode.OK,
                    true
                );
            }
            catch (Exception ex)
            {
                return UsersService<bool>.BadResult(
                    "Błąd podczas aktualizacji użytkownika",
                    (int)HttpStatusCode.InternalServerError,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<UsersService<bool>> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return UsersService<bool>.BadResult(
                        "Użytkownik nie został znaleziony",
                        (int)HttpStatusCode.NotFound
                    );
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return UsersService<bool>.GoodResult(
                    "Użytkownik usunięty pomyślnie",
                    (int)HttpStatusCode.OK,
                    true
                );
            }
            catch (Exception ex)
            {
                return UsersService<bool>.BadResult(
                    "Błąd podczas usuwania użytkownika",
                    (int)HttpStatusCode.InternalServerError,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<UsersService<bool>> UpdateUserStatusAsync(int id, bool isActive)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return UsersService<bool>.BadResult(
                        "Użytkownik nie został znaleziony",
                        (int)HttpStatusCode.NotFound
                    );
                }

                

                await _context.SaveChangesAsync();

                return UsersService<bool>.GoodResult(
                    "Status użytkownika zaktualizowany pomyślnie",
                    (int)HttpStatusCode.OK,
                    true
                );
            }
            catch (Exception ex)
            {
                return UsersService<bool>.BadResult(
                    "Błąd podczas aktualizacji statusu użytkownika",
                    (int)HttpStatusCode.InternalServerError,
                    new List<string> { ex.Message }
                );
            }
        }
    }
}