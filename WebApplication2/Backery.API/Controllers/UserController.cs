using Microsoft.AspNetCore.Mvc;
using WebApplication2.DTO;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

       
        [HttpGet]
        public async Task<ActionResult<PagedResult<UserResponseDto>>> GetUsers([FromQuery] UserQueryDto query)
        {
            
            if (query.PageSize > 100)
                query.PageSize = 100;
            if (query.PageSize < 1)
                query.PageSize = 10;
            if (query.Page < 1)
                query.Page = 1;

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
                return CreatedAtAction(
                    nameof(GetUser),
                    new { id = createdUser.UserId },
                    createdUser
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
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
                    return NotFound($"User with ID {id} not found");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid user ID");

            var success = await _userService.DeleteUserAsync(id);
            if (!success)
                return NotFound($"User with ID {id} not found");

            return NoContent();
        }

        
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] bool isActive)
        {
            if (id <= 0)
                return BadRequest("Invalid user ID");

            var success = await _userService.UpdateUserStatusAsync(id, isActive);
            if (!success)
                return NotFound($"User with ID {id} not found");

            return NoContent();
        }
    }
}