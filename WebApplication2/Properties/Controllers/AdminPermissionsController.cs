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
    public class AdminPermissionsController : ControllerBase
    {
        private readonly IAdminPermissionsService _adminPermissionsService;

        public AdminPermissionsController(IAdminPermissionsService adminPermissionsService)
        {
            _adminPermissionsService = adminPermissionsService;
        }

        // GET: api/AdminPermissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminPermission>>> GetAdminPermissions()
        {
            var permissions = await _adminPermissionsService.GetAllAdminPermissionsAsync();
            return Ok(permissions);
        }

        // GET: api/AdminPermissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminPermission>> GetAdminPermission(int id)
        {
            var permission = await _adminPermissionsService.GetAdminPermissionByIdAsync(id);
            if (permission == null)
            {
                return NotFound();
            }
            return Ok(permission);
        }

        // POST: api/AdminPermissions
        [HttpPost]
        public async Task<ActionResult<AdminPermission>> CreateAdminPermission(AdminPermission permission)
        {
            var createdPermission = await _adminPermissionsService.CreateAdminPermissionAsync(permission);
            return CreatedAtAction(nameof(GetAdminPermission), new { id = createdPermission.PermissionId }, createdPermission);
        }

        // PUT: api/AdminPermissions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdminPermission(int id, AdminPermission permission)
        {
            var result = await _adminPermissionsService.UpdateAdminPermissionAsync(id, permission);
            if (!result)
            {
                return BadRequest();
            }
            return NoContent();
        }

        // DELETE: api/AdminPermissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminPermission(int id)
        {
            var result = await _adminPermissionsService.DeleteAdminPermissionAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}