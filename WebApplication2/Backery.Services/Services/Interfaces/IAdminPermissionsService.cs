using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminPermissionsService
    {

        Task<ServiceResult<IEnumerable<AdminPermissionListItemDto>>> GetAllAdminPermissionsDtoAsync();
        Task<ServiceResult<AdminPermissionResponseDto>> GetAdminPermissionByIdDtoAsync(int id);
        Task<ServiceResult<AdminPermissionResponseDto>> CreateAdminPermissionAsync(CreateAdminPermissionDto createDto);
        Task<ServiceResult<object>> UpdateAdminPermissionAsync(int id, UpdateAdminPermissionDto updateDto);
        Task<ServiceResult<object>> DeleteAdminPermissionAsync(int id);
        Task<bool> AdminPermissionExistsAsync(int id);
    }
}
