using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services;
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AdminRolePermissionListItemDto>>> GetAdminRolePermissions()
        {
            var result = await _adminRolePermissionsService.GetAllAdminRolePermissionsDtoAsync();

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/AdminRolePermissions/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminRolePermissionListItemDto>> GetAdminRolePermission(int id)
        {
            var result = await _adminRolePermissionsService.GetAdminRolePermissionByIdDtoAsync(id);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // POST: api/AdminRolePermissions
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminRolePermissionListItemDto>> CreateAdminRolePermission(CreateAdminRolePermissionDto createDto)
        {
            var result = await _adminRolePermissionsService.CreateAdminRolePermissionAsync(createDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

       
        

        // DELETE: api/AdminRolePermissions/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAdminRolePermission(int id)
        {
            var result = await _adminRolePermissionsService.DeleteAdminRolePermissionAsync(id);

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