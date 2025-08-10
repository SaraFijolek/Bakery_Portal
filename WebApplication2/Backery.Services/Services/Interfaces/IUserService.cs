using System.Collections.Generic;
using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IUserService
    {
        Task<UsersService<PagedResult<UserResponseDto>>> GetUsersAsync(UserQueryDto query);
        Task<UsersService<UserResponseDto?>> GetUserByIdAsync(int id);
        Task<UsersService<UserResponseDto>> CreateUserAsync(UserCreateDto userCreateDto);
        Task<UsersService<bool>> UpdateUserAsync(int id, UserUpdateDto userUpdateDto);
        Task<UsersService<bool>> DeleteUserAsync(int id);
        Task<UsersService<bool>> UpdateUserStatusAsync(int id, bool isActive);
    }
}
