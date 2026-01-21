using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<ReportedContentDto>>> GetReportedContents()
        {
            var result = await _reportedContentsService.GetReportedContentsAsync();
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/ReportedContents/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<ReportedContentDto>> GetReportedContent(int id)
        {
            var result = await _reportedContentsService.GetReportedContentAsync(id);
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // POST: api/ReportedContents
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<ReportedContentDto>> CreateReportedContent(CreateReportedContentDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = ModelState.Values
                          .SelectMany(v => v.Errors)
                          .Select(e => e.ErrorMessage)
                          .ToList()
                });

            var result = await _reportedContentsService.CreateReportedContentAsync(createDto);
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // PUT: api/ReportedContents/5
        [HttpPut("{id}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<ReportedContentDto>> UpdateReportedContent(int id, UpdateReportedContentDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = ModelState.Values
                           .SelectMany(v => v.Errors)
                           .Select(e => e.ErrorMessage)
                           .ToList()
                });

            var result = await _reportedContentsService.UpdateReportedContentAsync(id, updateDto);

            if (!result.Success)
                return StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });

            return StatusCode(result.StatusCode, result.Data);


        }

        // DELETE: api/ReportedContents/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReportedContent(int id)
        {
            var result = await _reportedContentsService.DeleteReportedContentAsync(id);
            if (result.Success)
                return StatusCode(result.StatusCode, new
                {
                    success = true,
                    message = result.Message,
                    data = result.Data
                });

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }
    }
}
