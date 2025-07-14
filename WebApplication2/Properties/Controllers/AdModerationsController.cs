using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult<IEnumerable<AdModeration>>> GetAdModerations()
        {
            var moderations = await _adModerationsService.GetAdModerationsAsync();
            return Ok(moderations);
        }

        // GET: api/AdModerations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdModeration>> GetAdModeration(int id)
        {
            var moderation = await _adModerationsService.GetAdModerationByIdAsync(id);
            if (moderation == null)
                return NotFound();

            return Ok(moderation);
        }

        // POST: api/AdModerations
        [HttpPost]
        public async Task<ActionResult<AdModeration>> CreateAdModeration(AdModeration moderation)
        {
            var createdModeration = await _adModerationsService.CreateAdModerationAsync(moderation);
            return CreatedAtAction(nameof(GetAdModeration), new { id = createdModeration.ModerationId }, createdModeration);
        }

        // PUT: api/AdModerations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdModeration(int id, AdModeration moderation)
        {
            try
            {
                await _adModerationsService.UpdateAdModerationAsync(id, moderation);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/AdModerations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdModeration(int id)
        {
            var deleted = await _adModerationsService.DeleteAdModerationAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
