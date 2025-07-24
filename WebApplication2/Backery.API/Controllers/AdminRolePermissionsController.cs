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
    public class AdminRolePermissionsController : ControllerBase
    {
        private readonly IAdminRolePermissionsService _adminRolePermissionsService;

        public AdminRolePermissionsController(IAdminRolePermissionsService adminRolePermissionsService)
        {
            _adminRolePermissionsService = adminRolePermissionsService;
        }

        // GET: api/AdminRolePermissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminRolePermissionListItemDto>>> GetAdminRolePermissions()
        {
            var rolePermissions = await _adminRolePermissionsService.GetAllAdminRolePermissionsDtoAsync();
            return Ok(rolePermissions);
        }

        // GET: api/AdminRolePermissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminRolePermissionResponseDto>> GetAdminRolePermission(int id)
        {
            var rolePermission = await _adminRolePermissionsService.GetAdminRolePermissionByIdDtoAsync(id);
            if (rolePermission == null)
                return NotFound();

            return Ok(rolePermission);
        }

        // POST: api/AdminRolePermissions
        [HttpPost]
        public async Task<ActionResult<AdminRolePermissionResponseDto>> CreateAdminRolePermission(CreateAdminRolePermissionDto createDto)
        {
            var createdRolePermission = await _adminRolePermissionsService.CreateAdminRolePermissionAsync(createDto);
            return CreatedAtAction(nameof(GetAdminRolePermission), new { id = createdRolePermission.RolePermissionId }, createdRolePermission);
        }

        // PUT: api/AdminRolePermissions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdminRolePermission(int id, UpdateAdminRolePermissionDto updateDto)
        {
            var result = await _adminRolePermissionsService.UpdateAdminRolePermissionAsync(id, updateDto);
            if (!result)
                return BadRequest();

            return NoContent();
        }

        // DELETE: api/AdminRolePermissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminRolePermission(int id)
        {
            var result = await _adminRolePermissionsService.DeleteAdminRolePermissionAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
