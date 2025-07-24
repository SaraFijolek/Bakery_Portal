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
        public async Task<ActionResult<List<AdminSessionDto>>> GetAdminSessions()
        {
            var sessions = await _adminsSessionsService.GetAllAdminSessionsAsync();
            return Ok(sessions);
        }

        // GET: api/AdminsSessions/list (optimized for listing without full navigation data)
        [HttpGet("list")]
        public async Task<ActionResult<List<AdminSessionListDto>>> GetAdminSessionsList()
        {
            var sessions = await _adminsSessionsService.GetAllAdminSessionsListAsync();
            return Ok(sessions);
        }

        // GET: api/AdminsSessions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminSessionDto>> GetAdminSession(int id)
        {
            var session = await _adminsSessionsService.GetAdminSessionByIdAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            return Ok(session);
        }

        // POST: api/AdminsSessions
        [HttpPost]
        public async Task<ActionResult<AdminSessionDto>> CreateAdminSession(CreateAdminSessionDto sessionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdSession = await _adminsSessionsService.CreateAdminSessionAsync(sessionDto);
            return CreatedAtAction(nameof(GetAdminSession), new { id = createdSession.SessionId }, createdSession);
        }

        // PUT: api/AdminsSessions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdminSession(int id, UpdateAdminSessionDto sessionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _adminsSessionsService.UpdateAdminSessionAsync(id, sessionDto);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/AdminsSessions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminSession(int id)
        {
            var result = await _adminsSessionsService.DeleteAdminSessionAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
