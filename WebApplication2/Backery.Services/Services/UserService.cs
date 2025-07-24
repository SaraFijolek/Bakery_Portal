using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<UserResponseDto>> GetUsersAsync(UserQueryDto query)
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

            return new PagedResult<UserResponseDto>
            {
                Items = users,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
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

            return user;
        }

        public async Task<UserResponseDto> CreateUserAsync(UserCreateDto userCreateDto)
        {
            
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userCreateDto.Email);

            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists");

            
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

            return new UserResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Phone = user.Phone
            };
        }

        public async Task<bool> UpdateUserAsync(int id, UserUpdateDto userUpdateDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

           
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userUpdateDto.Email && u.UserId != id);

            if (existingUser != null)
                throw new InvalidOperationException("Email is already taken by another user");

            user.Name = userUpdateDto.Name;
            user.Email = userUpdateDto.Email;
            user.Phone = userUpdateDto.Phone;
           

            
            if (!string.IsNullOrEmpty(userUpdateDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userUpdateDto.Password);
            }

            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

           

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserStatusAsync(int id, bool isActive)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

           

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
