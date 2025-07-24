using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminAuditLogsService
    {
        Task<List<AdminAuditLogListItemDto>> GetAdminAuditLogsDtoAsync();
        Task<AdminAuditLogResponseDto> GetAdminAuditLogByIdDtoAsync(int id);
        Task<AdminAuditLogResponseDto> CreateAdminAuditLogAsync(CreateAdminAuditLogDto createDto);
        Task<bool> UpdateAdminAuditLogAsync(int id, UpdateAdminAuditLogDto updateDto);
        Task<bool> DeleteAdminAuditLogAsync(int id);
        Task<bool> AdminAuditLogExistsAsync(int id);
    }
}
