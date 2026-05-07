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
    public class AdminsController : ControllerBase
    {
        private readonly IAdminsService _adminsService;

        public AdminsController(IAdminsService adminsService)
        {
            _adminsService = adminsService;
        }

        // GET: api/Admins
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdmins()
        {
            var result = await _adminsService.GetAllAdminsAsync();

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/Admins/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdmin(string id)
        {
            var result = await _adminsService.GetAdminByIdAsync(id);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // POST: api/Admins
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdmin(CreateAdminDto createDto)
        {
            var result = await _adminsService.CreateAdminAsync(createDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // PUT: api/Admins/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAdmin(string id, UpdateAdminDto updateDto)
        {
            var result = await _adminsService.UpdateAdminAsync(id, updateDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // DELETE: api/Admins/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            var result = await _adminsService.DeleteAdminAsync(id);

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