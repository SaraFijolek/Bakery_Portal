using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminAuditLogsController : ControllerBase
    {
        private readonly IAdminAuditLogsService _adminAuditLogsService;

        public AdminAuditLogsController(IAdminAuditLogsService adminAuditLogsService)
        {
            _adminAuditLogsService = adminAuditLogsService;
        }

        // GET: api/AdminAuditLogs
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AdminAuditLogDto>>> GetAdminAuditLogs()
        {
            var result = await _adminAuditLogsService.GetAdminAuditLogsDtoAsync();

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/AdminAuditLogs/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminAuditLogDto>> GetAdminAuditLog(int id)
        {
            var result = await _adminAuditLogsService.GetAdminAuditLogByIdDtoAsync(id);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // POST: api/AdminAuditLogs
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminAuditLogDto>> CreateAdminAuditLog(CreateAdminAuditLogDto createDto)
        {
            var result = await _adminAuditLogsService.CreateAdminAuditLogAsync(createDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // PUT: api/AdminAuditLogs/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAdminAuditLog(int id, UpdateAdminAuditLogDto updateDto)
        {
            var result = await _adminAuditLogsService.UpdateAdminAuditLogAsync(id, updateDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // DELETE: api/AdminAuditLogs/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAdminAuditLog(int id)
        {
            var result = await _adminAuditLogsService.DeleteAdminAuditLogAsync(id);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }
    }
}