using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminAuditLogsService
    {
        Task<List<AdminAuditLog>> GetAdminAuditLogsAsync();
        Task<AdminAuditLog?> GetAdminAuditLogByIdAsync(int id);
        Task<AdminAuditLog> CreateAdminAuditLogAsync(AdminAuditLog log);
        Task<bool> UpdateAdminAuditLogAsync(int id, AdminAuditLog log);
        Task<bool> DeleteAdminAuditLogAsync(int id);
        Task<bool> AdminAuditLogExistsAsync(int id);
    }
}
