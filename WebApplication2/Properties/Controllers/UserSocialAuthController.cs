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
    public class UserSocialAuthController : ControllerBase
    {
        private readonly IUserSocialAuthService _userSocialAuthService;

        public UserSocialAuthController(IUserSocialAuthService userSocialAuthService)
        {
            _userSocialAuthService = userSocialAuthService;
        }

        // GET: api/UserSocialAuth
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserSocialAuth>>> GetUserSocialAuths()
        {
            var auths = await _userSocialAuthService.GetUserSocialAuthsAsync();
            return Ok(auths);
        }

        // GET: api/UserSocialAuth/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserSocialAuth>> GetUserSocialAuth(int id)
        {
            var auth = await _userSocialAuthService.GetUserSocialAuthByIdAsync(id);
            if (auth == null)
                return NotFound();

            return Ok(auth);
        }

        // POST: api/UserSocialAuth
        [HttpPost]
        public async Task<ActionResult<UserSocialAuth>> CreateUserSocialAuth(UserSocialAuth auth)
        {
            var createdAuth = await _userSocialAuthService.CreateUserSocialAuthAsync(auth);
            return CreatedAtAction(nameof(GetUserSocialAuth), new { id = createdAuth.SocialAuthId }, createdAuth);
        }

        // PUT: api/UserSocialAuth/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserSocialAuth(int id, UserSocialAuth auth)
        {
            var success = await _userSocialAuthService.UpdateUserSocialAuthAsync(id, auth);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        // DELETE: api/UserSocialAuth/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserSocialAuth(int id)
        {
            var success = await _userSocialAuthService.DeleteUserSocialAuthAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
