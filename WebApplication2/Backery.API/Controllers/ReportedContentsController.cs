using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.DTO;
using WebApplication2.Models;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;


namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportedContentsController : ControllerBase
    {
        private readonly IReportedContentsService _reportedContentsService;

        public ReportedContentsController(IReportedContentsService reportedContentsService)
        {
            _reportedContentsService = reportedContentsService;
        }

        // GET: api/ReportedContents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportedContentDto>>> GetReportedContents()
        {
            var reportedContents = await _reportedContentsService.GetReportedContentsAsync();
            return Ok(reportedContents);
        }

        // GET: api/ReportedContents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportedContentDto>> GetReportedContent(int id)
        {
            var report = await _reportedContentsService.GetReportedContentAsync(id);
            if (report == null)
                return NotFound();

            return Ok(report);
        }

        // POST: api/ReportedContents
        [HttpPost]
        public async Task<ActionResult<ReportedContentDto>> CreateReportedContent(CreateReportedContentDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdReport = await _reportedContentsService.CreateReportedContentAsync(createDto);
            return CreatedAtAction(nameof(GetReportedContent),
                new { id = createdReport.ReportId }, createdReport);
        }

        // PUT: api/ReportedContents/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ReportedContentDto>> UpdateReportedContent(int id, UpdateReportedContentDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedReport = await _reportedContentsService.UpdateReportedContentAsync(id, updateDto);
                return Ok(updatedReport);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/ReportedContents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReportedContent(int id)
        {
            var deleted = await _reportedContentsService.DeleteReportedContentAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
