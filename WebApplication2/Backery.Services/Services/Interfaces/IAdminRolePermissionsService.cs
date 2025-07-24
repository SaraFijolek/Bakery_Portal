using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminRolePermissionsService
    {
        Task<IEnumerable<AdminRolePermissionListItemDto>> GetAllAdminRolePermissionsDtoAsync();
        Task<AdminRolePermissionResponseDto?> GetAdminRolePermissionByIdDtoAsync(int id);
        Task<AdminRolePermissionResponseDto> CreateAdminRolePermissionAsync(CreateAdminRolePermissionDto createDto);
        Task<bool> UpdateAdminRolePermissionAsync(int id, UpdateAdminRolePermissionDto updateDto);
        Task<bool> DeleteAdminRolePermissionAsync(int id);
        Task<bool> AdminRolePermissionExistsAsync(int id);
    }
}
