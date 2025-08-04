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
        public async Task<ActionResult<PagedResult<UserResponseDto>>> GetUsers([FromQuery] UserQueryDto query)
        {
            // Walidacja paginacji
            if (query.PageSize > 100) query.PageSize = 100;
            if (query.PageSize < 1) query.PageSize = 10;
            if (query.Page < 1) query.Page = 1;

            var users = await _userService.GetUsersAsync(query);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid user ID");

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found");

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] UserCreateDto userCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdUser = await _userService.CreateUserAsync(userCreateDto);
                _logger.LogInformation("User created (ID: {UserId})", createdUser.UserId);

                return CreatedAtAction(
                    nameof(GetUser),
                    new { id = createdUser.UserId },
                    createdUser
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation during user creation for email={Email}", userCreateDto.Email);
                return BadRequest(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error during user creation for email={Email}", userCreateDto.Email);
                return StatusCode(500, "Database error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user creation.");
                return StatusCode(500, "Unexpected error occurred.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userUpdateDto)
        {
            if (id <= 0)
                return BadRequest("Invalid user ID");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var success = await _userService.UpdateUserAsync(id, userUpdateDto);
                if (!success)
                {
                    _logger.LogInformation("Update failed: User with ID {UserId} not found", id);
                    return NotFound($"User with ID {id} not found");
                }

                _logger.LogInformation("User updated (ID: {UserId})", id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation during user update (ID: {UserId})", id);
                return BadRequest(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error during user update (ID: {UserId})", id);
                return StatusCode(500, "Database error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user update (ID: {UserId})", id);
                return StatusCode(500, "Unexpected error occurred.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid user ID");

            try
            {
                var success = await _userService.DeleteUserAsync(id);
                if (!success)
                {
                    _logger.LogInformation("Delete failed: User with ID {UserId} not found", id);
                    return NotFound($"User with ID {id} not found");
                }

                _logger.LogInformation("User deleted (ID: {UserId})", id);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error during user deletion (ID: {UserId})", id);
                return StatusCode(500, "Database error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user deletion (ID: {UserId})", id);
                return StatusCode(500, "Unexpected error occurred.");
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] bool isActive)
        {
            if (id <= 0)
                return BadRequest("Invalid user ID");

            try
            {
                var success = await _userService.UpdateUserStatusAsync(id, isActive);
                if (!success)
                {
                    _logger.LogInformation("Status update failed: User with ID {UserId} not found", id);
                    return NotFound($"User with ID {id} not found");
                }

                _logger.LogInformation("User status updated (ID: {UserId}) -> IsActive={IsActive}", id, isActive);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error during status update (ID: {UserId})", id);
                return StatusCode(500, "Database error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during status update (ID: {UserId})", id);
                return StatusCode(500, "Unexpected error occurred.");
            }
        }
    }
}