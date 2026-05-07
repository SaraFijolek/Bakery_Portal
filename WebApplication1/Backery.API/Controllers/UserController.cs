using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetUsers([FromQuery] UserQueryDto query)
        {
            
            if (query.PageSize > 100) query.PageSize = 100;
            if (query.PageSize < 1) query.PageSize = 10;
            if (query.Page < 1) query.Page = 1;

            var result = await _userService.GetUsersAsync(query);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid user ID",
                    errors = new List<string> { "User ID cannot be null or empty" }
                });

            var result = await _userService.GetUserByIdAsync(id);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> CreateUser([FromBody] UserCreateDto userCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });

            var result = await _userService.CreateUserAsync(userCreateDto);

            if (result.Success)
            {
                _logger.LogInformation("User created (ID: {UserId})", result.Data?.UserId);
                return StatusCode(result.StatusCode, result.Data);
            }

            _logger.LogWarning("User creation failed for email={Email}: {Message}", userCreateDto.Email, result.Message);
            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles ="Admin,User")]
        public async Task<ActionResult> UpdateUser(string id, [FromBody] UserUpdateDto userUpdateDto)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid user ID",
                    errors = new List<string> { "User ID cannot be null or empty" }
                });

            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });

            var result = await _userService.UpdateUserAsync(id, userUpdateDto);

            if (result.Success)
            {
                _logger.LogInformation("User updated (ID: {UserId})", id);
                return StatusCode(result.StatusCode, new
                {
                    success = true,
                    message = result.Message,
                    data = result.Data
                });
            }

            _logger.LogWarning("User update failed (ID: {UserId}): {Message}", id, result.Message);
            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid user ID",
                    errors = new List<string> { "User ID cannot be null or empty" }
                });

            var result = await _userService.DeleteUserAsync(id);

            if (result.Success)
            {
                _logger.LogInformation("User deleted (ID: {UserId})", id);
                return StatusCode(result.StatusCode, new
                {
                    success = true,
                    message = result.Message,
                    data = result.Data
                });
            }

            _logger.LogWarning("User deletion failed (ID: {UserId}): {Message}", id, result.Message);
            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles =  "Admin,User")]
        public async Task<ActionResult> UpdateUserStatus(string id, [FromBody] bool isActive)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid user ID",
                    errors = new List<string> { "User ID cannot be null or empty" }
                });

            var result = await _userService.UpdateUserStatusAsync(id, isActive);

            if (result.Success)
            {
                _logger.LogInformation("User status updated (ID: {UserId}) -> IsActive={IsActive}", id, isActive);
                return StatusCode(result.StatusCode, new
                {
                    success = true,
                    message = result.Message,
                    data = result.Data
                });
            }

            _logger.LogWarning("User status update failed (ID: {UserId}): {Message}", id, result.Message);
            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }
    }
}