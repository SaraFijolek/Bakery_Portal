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
    public class UserModerationsController : ControllerBase
    {
        private readonly IUserModerationsService _userModerationsService;

        public UserModerationsController(IUserModerationsService userModerationsService)
        {
            _userModerationsService = userModerationsService;
        }

        // GET: api/UserModerations
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetUserModerations()
        {
            var result = await _userModerationsService.GetUserModerationsAsync();

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/UserModerations/list
        [HttpGet("list")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetUserModerationsList()
        {
            var result = await _userModerationsService.GetUserModerationsListAsync();

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/UserModerations/active
        [HttpGet("active")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetActiveUserModerations()
        {
            var result = await _userModerationsService.GetActiveUserModerationsAsync();

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/UserModerations/user/5
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetUserModerationsByUserId(string userId)
        {
            var result = await _userModerationsService.GetUserModerationsByUserIdAsync(userId);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/UserModerations/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetUserModeration(int id)
        {
            var result = await _userModerationsService.GetUserModerationByIdAsync(id);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // POST: api/UserModerations
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> CreateUserModeration(CreateUserModerationDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid model state",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });

            var result = await _userModerationsService.CreateUserModerationAsync(createDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // PUT: api/UserModerations/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdateUserModeration(int id, UpdateUserModerationDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid model state",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });

            var result = await _userModerationsService.UpdateUserModerationAsync(id, updateDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // DELETE: api/UserModerations/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserModeration(int id)
        {
            var result = await _userModerationsService.DeleteUserModerationAsync(id);

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