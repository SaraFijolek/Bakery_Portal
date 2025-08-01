﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<List<AdminAuditLogListItemDto>>> GetAdminAuditLogs()
        {
            var logs = await _adminAuditLogsService.GetAdminAuditLogsDtoAsync();
            return Ok(logs);
        }

        // GET: api/AdminAuditLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminAuditLogResponseDto>> GetAdminAuditLog(int id)
        {
            var log = await _adminAuditLogsService.GetAdminAuditLogByIdDtoAsync(id);
            if (log == null)
                return NotFound();

            return Ok(log);
        }

        // POST: api/AdminAuditLogs
        [HttpPost]
        public async Task<ActionResult<AdminAuditLogResponseDto>> CreateAdminAuditLog(CreateAdminAuditLogDto createDto)
        {
            var createdLog = await _adminAuditLogsService.CreateAdminAuditLogAsync(createDto);
            return CreatedAtAction(nameof(GetAdminAuditLog), new { id = createdLog.LogId }, createdLog);
        }

        // PUT: api/AdminAuditLogs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdminAuditLog(int id, UpdateAdminAuditLogDto updateDto)
        {
            var success = await _adminAuditLogsService.UpdateAdminAuditLogAsync(id, updateDto);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        // DELETE: api/AdminAuditLogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminAuditLog(int id)
        {
            var success = await _adminAuditLogsService.DeleteAdminAuditLogAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
