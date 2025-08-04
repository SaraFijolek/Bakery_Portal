using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;
using WebApplication2.DTO;


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
        public async Task<ActionResult<IEnumerable<AdminDto>>> GetAdmins()
        {
            var admins = await _adminsService.GetAllAdminsAsync();
            return Ok(admins);
        }

        // GET: api/Admins/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminDto>> GetAdmin(int id)
        {
            var admin = await _adminsService.GetAdminByIdAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return Ok(admin);
        }

        // POST: api/Admins
        [HttpPost]
        public async Task<ActionResult<AdminDto>> CreateAdmin(CreateAdminDto createDto)
        {
            var createdAdmin = await _adminsService.CreateAdminAsync(createDto);
            return CreatedAtAction(nameof(GetAdmin), new { id = createdAdmin.AdminId }, createdAdmin);
        }

        // PUT: api/Admins/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdmin(int id, UpdateAdminDto updateDto)
        {
            var result = await _adminsService.UpdateAdminAsync(id, updateDto);
            if (!result)
            {
                return BadRequest();
            }
            return NoContent();
        }

        // DELETE: api/Admins/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var result = await _adminsService.DeleteAdminAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
