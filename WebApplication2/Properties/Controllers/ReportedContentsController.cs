using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;


namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportedContentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportedContentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ReportedContents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportedContent>>> GetReportedContents()
        {
            return await _context.ReportedContents
                .Include(r => r.ReporterUser)
                .Include(r => r.Admin)
                .ToListAsync();
        }

        // GET: api/ReportedContents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportedContent>> GetReportedContent(int id)
        {
            var report = await _context.ReportedContents
                .Include(r => r.ReporterUser)
                .Include(r => r.Admin)
                .FirstOrDefaultAsync(r => r.ReportId == id);

            if (report == null)
                return NotFound();

            return report;
        }

        // POST: api/ReportedContents
        [HttpPost]
        public async Task<ActionResult<ReportedContent>> CreateReportedContent(ReportedContent report)
        {
            _context.ReportedContents.Add(report);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReportedContent), new { id = report.ReportId }, report);
        }

        // PUT: api/ReportedContents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReportedContent(int id, ReportedContent report)
        {
            if (id != report.ReportId)
                return BadRequest();

            _context.Entry(report).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportedContentExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/ReportedContents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReportedContent(int id)
        {
            var report = await _context.ReportedContents.FindAsync(id);
            if (report == null)
                return NotFound();

            _context.ReportedContents.Remove(report);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReportedContentExists(int id)
        {
            return _context.ReportedContents.Any(e => e.ReportId == id);
        }
    }
}
