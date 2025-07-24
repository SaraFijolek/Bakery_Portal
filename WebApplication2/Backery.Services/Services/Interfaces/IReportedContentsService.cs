using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IReportedContentsService
    {
        Task<IEnumerable<ReportedContentReadDto>> GetReportedContentsAsync();
        Task<ReportedContentReadDto?> GetReportedContentAsync(int id);
        Task<ReportedContentReadDto> CreateReportedContentAsync(CreateReportedContentDto createDto);
        Task<ReportedContentReadDto> UpdateReportedContentAsync(int id, UpdateReportedContentDto updateDto);
        Task<bool> DeleteReportedContentAsync(int id);
        Task<bool> ReportedContentExistsAsync(int id);
    }
}
