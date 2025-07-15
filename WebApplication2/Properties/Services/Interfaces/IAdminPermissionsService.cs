using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminPermissionsService
    {
        Task<IEnumerable<AdminPermission>> GetAllAdminPermissionsAsync();
        Task<AdminPermission?> GetAdminPermissionByIdAsync(int id);
        Task<AdminPermission> CreateAdminPermissionAsync(AdminPermission permission);
        Task<bool> UpdateAdminPermissionAsync(int id, AdminPermission permission);
        Task<bool> DeleteAdminPermissionAsync(int id);
        Task<bool> AdminPermissionExistsAsync(int id);
    }
}
