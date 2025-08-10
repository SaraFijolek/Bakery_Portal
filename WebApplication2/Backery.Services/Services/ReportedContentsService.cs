using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{

    public class ReportedContentService<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static ReportedContentService<T> GoodResult(
            string message,
            int statusCode,
            T? data = default)
            => new ReportedContentService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = true,
                Data = data
            };

        public static ReportedContentService<T> BadResult(
            string message,
            int statusCode,
            List<string>? errors = null,
            T? data = default)
            =>
            new ReportedContentService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = false,
                Errors = errors ?? new List<string>(),
                Data = data
            };
    }
    public class ReportedContentsService : IReportedContentsService
    {
        private readonly AppDbContext _context;

        public ReportedContentsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ReportedContentService<IEnumerable<ReportedContentReadDto>>> GetReportedContentsAsync()
        {
            try
            {
                var reportedContents = await _context.ReportedContents
                    .Include(r => r.ReporterUser)
                    .Include(r => r.Admin)
                    .ToListAsync();

                var reportedContentDtos = reportedContents.Select(r => new ReportedContentReadDto
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

                return ReportedContentService<IEnumerable<ReportedContentReadDto>>.GoodResult(
                    "Reported contents retrieved successfully",
                    200,
                    reportedContentDtos);
            }
            catch (Exception ex)
            {
                return ReportedContentService<IEnumerable<ReportedContentReadDto>>.BadResult(
                    "Failed to retrieve reported contents",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ReportedContentService<ReportedContentReadDto>> GetReportedContentAsync(int id)
        {
            try
            {
                var reportedContent = await _context.ReportedContents
                    .Include(r => r.ReporterUser)
                    .Include(r => r.Admin)
                    .FirstOrDefaultAsync(r => r.ReportId == id);

                if (reportedContent == null)
                {
                    return ReportedContentService<ReportedContentReadDto>.BadResult(
                        $"Reported content with ID {id} not found",
                        404);
                }

                var reportedContentDto = new ReportedContentReadDto
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

                return ReportedContentService<ReportedContentReadDto>.GoodResult(
                    "Reported content retrieved successfully",
                    200,
                    reportedContentDto);
            }
            catch (Exception ex)
            {
                return ReportedContentService<ReportedContentReadDto>.BadResult(
                    "Failed to retrieve reported content",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ReportedContentService<ReportedContentReadDto>> CreateReportedContentAsync(CreateReportedContentDto createDto)
        {
            try
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

                var reportedContentDto = new ReportedContentReadDto
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

                return ReportedContentService<ReportedContentReadDto>.GoodResult(
                    "Reported content created successfully",
                    201,
                    reportedContentDto);
            }
            catch (Exception ex)
            {
                return ReportedContentService<ReportedContentReadDto>.BadResult(
                    "Failed to create reported content",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ReportedContentService<ReportedContentReadDto>> UpdateReportedContentAsync(int id, UpdateReportedContentDto updateDto)
        {
            try
            {
                var existingReport = await _context.ReportedContents.FindAsync(id);
                if (existingReport == null)
                {
                    return ReportedContentService<ReportedContentReadDto>.BadResult(
                        $"Reported content with ID {id} not found",
                        404);
                }

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
                    {
                        return ReportedContentService<ReportedContentReadDto>.BadResult(
                            $"Reported content with ID {id} not found",
                            404);
                    }
                    throw;
                }

                // Reload with navigation properties
                var updatedReport = await _context.ReportedContents
                    .Include(r => r.ReporterUser)
                    .Include(r => r.Admin)
                    .FirstOrDefaultAsync(r => r.ReportId == id);

                var reportedContentDto = new ReportedContentReadDto
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

                return ReportedContentService<ReportedContentReadDto>.GoodResult(
                    "Reported content updated successfully",
                    200,
                    reportedContentDto);
            }
            catch (Exception ex)
            {
                return ReportedContentService<ReportedContentReadDto>.BadResult(
                    "Failed to update reported content",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ReportedContentService<bool>> DeleteReportedContentAsync(int id)
        {
            try
            {
                var report = await _context.ReportedContents.FindAsync(id);
                if (report == null)
                {
                    return ReportedContentService<bool>.BadResult(
                        $"Reported content with ID {id} not found",
                        404,
                        data: false);
                }

                _context.ReportedContents.Remove(report);
                await _context.SaveChangesAsync();

                return ReportedContentService<bool>.GoodResult(
                    "Reported content deleted successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return ReportedContentService<bool>.BadResult(
                    "Failed to delete reported content",
                    500,
                    new List<string> { ex.Message },
                    false);
            }
        }

        public async Task<bool> ReportedContentExistsAsync(int id)
        {
            return await _context.ReportedContents.AnyAsync(e => e.ReportId == id);
        }
    }
}
