using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminPermissionsService
    {

        Task<IEnumerable<AdminPermissionListItemDto>> GetAllAdminPermissionsDtoAsync();
        Task<AdminPermissionResponseDto?> GetAdminPermissionByIdDtoAsync(int id);
        Task<AdminPermissionResponseDto> CreateAdminPermissionAsync(CreateAdminPermissionDto createDto);
        Task<bool> UpdateAdminPermissionAsync(int id, UpdateAdminPermissionDto updateDto);
        Task<bool> DeleteAdminPermissionAsync(int id);
        Task<bool> AdminPermissionExistsAsync(int id);
    }
}
