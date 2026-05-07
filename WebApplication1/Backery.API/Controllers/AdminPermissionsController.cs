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
    public class AdminPermissionsController : ControllerBase
    {
        private readonly IAdminPermissionsService _adminPermissionsService;

        public AdminPermissionsController(IAdminPermissionsService adminPermissionsService)
        {
            _adminPermissionsService = adminPermissionsService;
        }

        // GET: api/AdminPermissions
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AdminPermissionListItemDto>>> GetAdminPermissions()
        {
            var result = await _adminPermissionsService.GetAllAdminPermissionsDtoAsync();

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/AdminPermissions/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminPermissionListItemDto>> GetAdminPermission(int id)
        {
            var result = await _adminPermissionsService.GetAdminPermissionByIdDtoAsync(id);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // POST: api/AdminPermissions
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminPermissionResponseDto>> CreateAdminPermission(CreateAdminPermissionDto createDto)
        {
            var result = await _adminPermissionsService.CreateAdminPermissionAsync(createDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // PUT: api/AdminPermissions/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAdminPermission(int id, UpdateAdminPermissionDto updateDto)
        {
            var result = await _adminPermissionsService.UpdateAdminPermissionAsync(id, updateDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // DELETE: api/AdminPermissions/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAdminPermission(int id)
        {
            var result = await _adminPermissionsService.DeleteAdminPermissionAsync(id);

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