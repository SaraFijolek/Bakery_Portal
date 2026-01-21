using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.DTO;
using WebApplication2.Models;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdModerationsController : ControllerBase
    {
        private readonly IAdModerationsService _adModerationsService;

        public AdModerationsController(IAdModerationsService adModerationsService)
        {
            _adModerationsService = adModerationsService;
        }

        // GET: api/AdModerations
        [HttpGet]
        [AllowAnonymous] 
        public async Task<ActionResult<IEnumerable<AdModerationDto>>> GetAdModerations()
        {
            var result = await _adModerationsService.GetAdModerationsAsync();
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/AdModerations/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<AdModerationDto>> GetAdModeration(int id)
        {
            var result = await _adModerationsService.GetAdModerationByIdAsync(id);
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // POST: api/AdModerations
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AdModerationDto>> CreateAdModeration(CreateAdModerationDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid model state",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });

            var result = await _adModerationsService.CreateAdModerationAsync(createDto);
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // PUT: api/AdModerations/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<AdModerationDto>> UpdateAdModeration(int id, UpdateAdModerationDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid model state",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
           
            
                var result = await _adModerationsService.UpdateAdModerationAsync(id, updateDto);
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });



        }

        // DELETE: api/AdModerations/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAdModeration(int id)
        {
            var result = await _adModerationsService.DeleteAdModerationAsync(id);
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
