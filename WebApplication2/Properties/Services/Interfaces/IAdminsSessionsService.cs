using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminsSessionsService
    {
        Task<IEnumerable<AdminSession>> GetAllAdminSessionsAsync();
        Task<AdminSession?> GetAdminSessionByIdAsync(int id);
        Task<AdminSession> CreateAdminSessionAsync(AdminSession session);
        Task<bool> UpdateAdminSessionAsync(int id, AdminSession session);
        Task<bool> DeleteAdminSessionAsync(int id);
        Task<bool> AdminSessionExistsAsync(int id);
    }
}
