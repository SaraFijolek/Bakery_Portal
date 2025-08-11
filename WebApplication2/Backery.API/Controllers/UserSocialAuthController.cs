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
    public class UserSocialAuthController : ControllerBase
    {
        private readonly IUserSocialAuthService _userSocialAuthService;

        public UserSocialAuthController(IUserSocialAuthService userSocialAuthService)
        {
            _userSocialAuthService = userSocialAuthService;
        }

        // GET: api/UserSocialAuth
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<UserSocialAuth>>> GetUserSocialAuths()
        {
            var result = await _userSocialAuthService.GetUserSocialAuthsAsync();

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/UserSocialAuth/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<UserSocialAuth>> GetUserSocialAuth(int id)
        {
            var result = await _userSocialAuthService.GetUserSocialAuthByIdAsync(id);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // POST: api/UserSocialAuth
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<UserSocialAuth>> CreateUserSocialAuth(CreateUserSocialAuthDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userSocialAuthService.CreateUserSocialAuthAsync(createDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // PUT: api/UserSocialAuth/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdateUserSocialAuth(int id, UpdateUserSocialAuthDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userSocialAuthService.UpdateUserSocialAuthAsync(id, updateDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // DELETE: api/UserSocialAuth/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserSocialAuth(int id)
        {
            var result = await _userSocialAuthService.DeleteUserSocialAuthAsync(id);

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