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
    public class AdminsController : ControllerBase
    {
        private readonly IAdminsService _adminsService;

        public AdminsController(IAdminsService adminsService)
        {
            _adminsService = adminsService;
        }

        // GET: api/Admins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
        {
            var admins = await _adminsService.GetAllAdminsAsync();
            return Ok(admins);
        }

        // GET: api/Admins/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdmin(int id)
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
        public async Task<ActionResult<Admin>> CreateAdmin(Admin admin)
        {
            var createdAdmin = await _adminsService.CreateAdminAsync(admin);
            return CreatedAtAction(nameof(GetAdmin), new { id = createdAdmin.AdminId }, createdAdmin);
        }

        // PUT: api/Admins/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdmin(int id, Admin admin)
        {
            var result = await _adminsService.UpdateAdminAsync(id, admin);
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
