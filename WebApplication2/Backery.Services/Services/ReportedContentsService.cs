using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
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

        public async Task<IEnumerable<ReportedContentReadDto>> GetReportedContentsAsync()
        {
            var reportedContents = await _context.ReportedContents
                .Include(r => r.ReporterUser)
                .Include(r => r.Admin)
                .ToListAsync();

            return reportedContents.Select(r => new ReportedContentReadDto
            {
                ReportId = r.ReportId,
                ReporterUserId = r.ReporterUserId,
                ReporterEmail = r.ReporterEmail,
                ContentType = r.ContentType,
                ContentId = r.ContentId,
                Reason = r.Reason,
                Description = r.Description,
                Status = r.Status,
                AdminId = r.AdminId,
                AdminNotes = r.AdminNotes,
                CreatedAt = r.CreatedAt,
                ResolvedAt = r.ResolvedAt,
               
            });
        }

        public async Task<ReportedContentReadDto?> GetReportedContentAsync(int id)
        {
            var reportedContent = await _context.ReportedContents
                .Include(r => r.ReporterUser)
                .Include(r => r.Admin)
                .FirstOrDefaultAsync(r => r.ReportId == id);

            if (reportedContent == null)
                return null;

            return new ReportedContentReadDto
            {
                ReportId = reportedContent.ReportId,
                ReporterUserId = reportedContent.ReporterUserId,
                ReporterEmail = reportedContent.ReporterEmail,
                ContentType = reportedContent.ContentType,
                ContentId = reportedContent.ContentId,
                Reason = reportedContent.Reason,
                Description = reportedContent.Description,
                Status = reportedContent.Status,
                AdminId = reportedContent.AdminId,
                AdminNotes = reportedContent.AdminNotes,
                CreatedAt = reportedContent.CreatedAt,
                ResolvedAt = reportedContent.ResolvedAt,
                
            };
        }

        public async Task<ReportedContentReadDto> CreateReportedContentAsync(CreateReportedContentDto createDto)
        {
            var reportedContent = new ReportedContent
            {
                ReporterUserId = createDto.ReporterUserId,
                ReporterEmail = createDto.ReporterEmail,
                ContentType = createDto.ContentType,
                ContentId = createDto.ContentId,
                Reason = createDto.Reason,
                Description = createDto.Description,
                Status = createDto.Status ?? "pending",
                AdminId = createDto.AdminId,
                AdminNotes = createDto.AdminNotes,
                CreatedAt = DateTime.Now,
                ResolvedAt = createDto.ResolvedAt
            };

            _context.ReportedContents.Add(reportedContent);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            var createdReport = await _context.ReportedContents
                .Include(r => r.ReporterUser)
                .Include(r => r.Admin)
                .FirstOrDefaultAsync(r => r.ReportId == reportedContent.ReportId);

            return new ReportedContentReadDto
            {
                ReportId = createdReport.ReportId,
                ReporterUserId = createdReport.ReporterUserId,
                ReporterEmail = createdReport.ReporterEmail,
                ContentType = createdReport.ContentType,
                ContentId = createdReport.ContentId,
                Reason = createdReport.Reason,
                Description = createdReport.Description,
                Status = createdReport.Status,
                AdminId = createdReport.AdminId,
                AdminNotes = createdReport.AdminNotes,
                CreatedAt = createdReport.CreatedAt,
                ResolvedAt = createdReport.ResolvedAt,
                
            };
        }

        public async Task<ReportedContentReadDto> UpdateReportedContentAsync(int id, UpdateReportedContentDto updateDto)
        {
            var existingReport = await _context.ReportedContents.FindAsync(id);
            if (existingReport == null)
                throw new KeyNotFoundException($"Reported content with ID {id} not found");

            existingReport.ReporterUserId = updateDto.ReporterUserId;
            existingReport.ReporterEmail = updateDto.ReporterEmail;
            existingReport.ContentType = updateDto.ContentType;
            existingReport.ContentId = updateDto.ContentId;
            existingReport.Reason = updateDto.Reason;
            existingReport.Description = updateDto.Description;
            existingReport.Status = updateDto.Status;
            existingReport.AdminId = updateDto.AdminId;
            existingReport.AdminNotes = updateDto.AdminNotes;
            existingReport.ResolvedAt = updateDto.ResolvedAt;

            _context.Entry(existingReport).State = EntityState.Modified;

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

            // Reload with navigation properties
            var updatedReport = await _context.ReportedContents
                .Include(r => r.ReporterUser)
                .Include(r => r.Admin)
                .FirstOrDefaultAsync(r => r.ReportId == id);

            return new ReportedContentReadDto
            {
                ReportId = updatedReport.ReportId,
                ReporterUserId = updatedReport.ReporterUserId,
                ReporterEmail = updatedReport.ReporterEmail,
                ContentType = updatedReport.ContentType,
                ContentId = updatedReport.ContentId,
                Reason = updatedReport.Reason,
                Description = updatedReport.Description,
                Status = updatedReport.Status,
                AdminId = updatedReport.AdminId,
                AdminNotes = updatedReport.AdminNotes,
                CreatedAt = updatedReport.CreatedAt,
                ResolvedAt = updatedReport.ResolvedAt,
                
            };
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
