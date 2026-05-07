using System.Collections.Generic;
using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IUserService
    {
        Task<UsersService<PagedResult<UserResponseDto>>> GetUsersAsync(UserQueryDto query);
        Task<UsersService<UserResponseDto?>> GetUserByIdAsync(string id);
        Task<UsersService<UserResponseDto>> CreateUserAsync(UserCreateDto userCreateDto);
        Task<UsersService<bool>> UpdateUserAsync(string id, UserUpdateDto userUpdateDto);
        Task<UsersService<bool>> DeleteUserAsync(string id);
        Task<UsersService<bool>> UpdateUserStatusAsync(string id, bool isActive);
    }
}
