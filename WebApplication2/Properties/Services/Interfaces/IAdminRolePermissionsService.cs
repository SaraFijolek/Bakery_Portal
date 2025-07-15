using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminRolePermissionsService
    {
        Task<IEnumerable<AdminRolePermission>> GetAllAdminRolePermissionsAsync();
        Task<AdminRolePermission?> GetAdminRolePermissionByIdAsync(int id);
        Task<AdminRolePermission> CreateAdminRolePermissionAsync(AdminRolePermission rolePermission);
        Task<bool> UpdateAdminRolePermissionAsync(int id, AdminRolePermission rolePermission);
        Task<bool> DeleteAdminRolePermissionAsync(int id);
        Task<bool> AdminRolePermissionExistsAsync(int id);
    }
}
