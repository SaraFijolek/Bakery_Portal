using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminsService
    {
        Task<IEnumerable<Admin>> GetAllAdminsAsync();
        Task<Admin?> GetAdminByIdAsync(int id);
        Task<Admin> CreateAdminAsync(Admin admin);
        Task<bool> UpdateAdminAsync(int id, Admin admin);
        Task<bool> DeleteAdminAsync(int id);
        Task<bool> AdminExistsAsync(int id);
    }
}
