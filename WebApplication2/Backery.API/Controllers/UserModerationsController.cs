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
    public class UserModerationsController : ControllerBase
    {
        private readonly IUserModerationsService _userModerationsService;

        public UserModerationsController(IUserModerationsService userModerationsService)
        {
            _userModerationsService = userModerationsService;
        }

        // GET: api/UserModerations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModerationDto>>> GetUserModerations()
        {
            var moderations = await _userModerationsService.GetUserModerationsAsync();
            return Ok(moderations);
        }

        // GET: api/UserModerations/list
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<UserModerationListDto>>> GetUserModerationsList()
        {
            var moderations = await _userModerationsService.GetUserModerationsListAsync();
            return Ok(moderations);
        }

        // GET: api/UserModerations/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<UserModerationDto>>> GetActiveUserModerations()
        {
            var moderations = await _userModerationsService.GetActiveUserModerationsAsync();
            return Ok(moderations);
        }

        // GET: api/UserModerations/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserModerationDto>>> GetUserModerationsByUserId(int userId)
        {
            var moderations = await _userModerationsService.GetUserModerationsByUserIdAsync(userId);
            return Ok(moderations);
        }

        // GET: api/UserModerations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModerationDto>> GetUserModeration(int id)
        {
            var moderation = await _userModerationsService.GetUserModerationByIdAsync(id);
            if (moderation == null)
                return NotFound();

            return Ok(moderation);
        }

        // POST: api/UserModerations
        [HttpPost]
        public async Task<ActionResult<UserModerationDto>> CreateUserModeration(CreateUserModerationDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdModeration = await _userModerationsService.CreateUserModerationAsync(createDto);
            return CreatedAtAction(nameof(GetUserModeration), new { id = createdModeration.ModerationId }, createdModeration);
        }

        // PUT: api/UserModerations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserModeration(int id, UpdateUserModerationDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _userModerationsService.UpdateUserModerationAsync(id, updateDto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/UserModerations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserModeration(int id)
        {
            var success = await _userModerationsService.DeleteUserModerationAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
