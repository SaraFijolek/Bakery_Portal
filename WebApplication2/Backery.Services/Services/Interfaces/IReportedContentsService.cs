using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IReportedContentsService
    {
        Task<ReportedContentService<IEnumerable<ReportedContentReadDto>>> GetReportedContentsAsync();
        Task<ReportedContentService<ReportedContentReadDto>> GetReportedContentAsync(int id);
        Task<ReportedContentService<ReportedContentReadDto>> CreateReportedContentAsync(CreateReportedContentDto createDto);
        Task<ReportedContentService<ReportedContentReadDto>> UpdateReportedContentAsync(int id, UpdateReportedContentDto updateDto);
        Task<ReportedContentService<bool>> DeleteReportedContentAsync(int id);
        Task<bool> ReportedContentExistsAsync(int id);
    }
}
