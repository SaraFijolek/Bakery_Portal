using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminAuditLogsService
    {
        Task<ResultsService<List<AdminAuditLogListItemDto>>> GetAdminAuditLogsDtoAsync();
        Task<ResultsService<AdminAuditLogResponseDto>> GetAdminAuditLogByIdDtoAsync(int id);
        Task<ResultsService<AdminAuditLogResponseDto>> CreateAdminAuditLogAsync(CreateAdminAuditLogDto createDto);
        Task<ResultsService<object>> UpdateAdminAuditLogAsync(int id, UpdateAdminAuditLogDto updateDto);
        Task<ResultsService<object>> DeleteAdminAuditLogAsync(int id);
        Task<bool> AdminAuditLogExistsAsync(int id);
    }
}
