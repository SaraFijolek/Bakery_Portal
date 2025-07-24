using System.Collections.Generic;
using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserResponseDto>> GetUsersAsync(UserQueryDto query);
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<UserResponseDto> CreateUserAsync(UserCreateDto userCreateDto);
        Task<bool> UpdateUserAsync(int id, UserUpdateDto userUpdateDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UpdateUserStatusAsync(int id, bool isActive);
    }
}
