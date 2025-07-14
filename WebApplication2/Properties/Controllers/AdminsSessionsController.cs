using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Properties.Data;
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

        // GET: api/AdminSessions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminSession>>> GetAdminSessions()
        {
            var sessions = await _adminsSessionsService.GetAllAdminSessionsAsync();
            return Ok(sessions);
        }

        // GET: api/AdminSessions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminSession>> GetAdminSession(int id)
        {
            var session = await _adminsSessionsService.GetAdminSessionByIdAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            return Ok(session);
        }

        // POST: api/AdminSessions
        [HttpPost]
        public async Task<ActionResult<AdminSession>> CreateAdminSession(AdminSession session)
        {
            var createdSession = await _adminsSessionsService.CreateAdminSessionAsync(session);
            return CreatedAtAction(nameof(GetAdminSession), new { id = createdSession.SessionId }, createdSession);
        }

        // PUT: api/AdminSessions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdminSession(int id, AdminSession session)
        {
            var result = await _adminsSessionsService.UpdateAdminSessionAsync(id, session);
            if (!result)
            {
                return BadRequest();
            }
            return NoContent();
        }

        // DELETE: api/AdminSessions/5
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
