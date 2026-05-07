using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminsSessionsController : ControllerBase
    {
        private readonly IAdminsSessionsService _adminsSessionsService;

        public AdminsSessionsController(IAdminsSessionsService adminsSessionsService)
        {
            _adminsSessionsService = adminsSessionsService;
        }

        // GET: api/AdminsSessions
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<AdminSessionDto>>> GetAdminSessions()
        {
            var result = await _adminsSessionsService.GetAllAdminSessionsAsync();

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/AdminsSessions/list (optimized for listing without full navigation data)
        [HttpGet("list")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<AdminSessionListDto>>> GetAdminSessionsList()
        {
            var result = await _adminsSessionsService.GetAllAdminSessionsListAsync();

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/AdminsSessions/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminSessionDto>> GetAdminSession(int id)
        {
            var result = await _adminsSessionsService.GetAdminSessionByIdAsync(id);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // POST: api/AdminsSessions
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminSessionDto>> CreateAdminSession(CreateAdminSessionDto sessionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _adminsSessionsService.CreateAdminSessionAsync(sessionDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // PUT: api/AdminsSessions/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAdminSession(int id, UpdateAdminSessionDto sessionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _adminsSessionsService.UpdateAdminSessionAsync(id, sessionDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // DELETE: api/AdminsSessions/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAdminSession(int id)
        {
            var result = await _adminsSessionsService.DeleteAdminSessionAsync(id);

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