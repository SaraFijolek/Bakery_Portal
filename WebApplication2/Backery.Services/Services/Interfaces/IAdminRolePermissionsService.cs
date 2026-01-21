using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminRolePermissionsService
    {
        Task<ResultService<IEnumerable<AdminRolePermissionListItemDto>>> GetAllAdminRolePermissionsDtoAsync();
        Task<ResultService<AdminRolePermissionResponseDto>> GetAdminRolePermissionByIdDtoAsync(int id);
        Task<ResultService<AdminRolePermissionResponseDto>> CreateAdminRolePermissionAsync(CreateAdminRolePermissionDto createDto);
        Task<ResultService<bool>> UpdateAdminRolePermissionAsync(int id, AdminRolePermission rolePermission);
        Task<ResultService<bool>> DeleteAdminRolePermissionAsync(int id);
        Task<bool> AdminRolePermissionExistsAsync(int id);
       
    }
}
