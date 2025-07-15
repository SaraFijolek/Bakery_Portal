using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IReportedContentsService
    {
        Task<IEnumerable<ReportedContent>>GetReportedContentsAsync();
        Task<ReportedContent?>GetReportedContentAsync(int id);
        Task<ReportedContent>CreateReportedContentAsync(ReportedContent report);
        Task<ReportedContent>UpdateReportedContentAsync(int id, ReportedContent report);
        Task<bool>DeleteReportedContentAsync(int id);
        Task<bool>ReportedContentExistsAsync(int id);
    }
}
