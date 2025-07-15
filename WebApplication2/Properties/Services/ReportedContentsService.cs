using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class ReportedContentsService : IReportedContentsService
    {
        private readonly AppDbContext _context;

        public ReportedContentsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReportedContent>> GetReportedContentsAsync()
        {
            return await _context.ReportedContents
                .Include(r => r.ReporterUser)
                .Include(r => r.Admin)
                .ToListAsync();
        }

        public async Task<ReportedContent?> GetReportedContentAsync(int id)
        {
            return await _context.ReportedContents
                .Include(r => r.ReporterUser)
                .Include(r => r.Admin)
                .FirstOrDefaultAsync(r => r.ReportId == id);
        }

        public async Task<ReportedContent> CreateReportedContentAsync(ReportedContent report)
        {
            _context.ReportedContents.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<ReportedContent> UpdateReportedContentAsync(int id, ReportedContent report)
        {
            if (id != report.ReportId)
                throw new ArgumentException("Report ID mismatch");

            _context.Entry(report).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ReportedContentExistsAsync(id))
                    throw new KeyNotFoundException($"Reported content with ID {id} not found");
                throw;
            }

            return report;
        }

        public async Task<bool> DeleteReportedContentAsync(int id)
        {
            var report = await _context.ReportedContents.FindAsync(id);
            if (report == null)
                return false;

            _context.ReportedContents.Remove(report);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReportedContentExistsAsync(int id)
        {
            return await _context.ReportedContents.AnyAsync(e => e.ReportId == id);
        }
    }
}
