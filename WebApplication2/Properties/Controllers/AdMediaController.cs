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
    public class AdMediaController : ControllerBase
    {
        private readonly IAdMadiaService _adMadiaService;

        public AdMediaController(IAdMadiaService adMadiaService)
        {
            _adMadiaService = adMadiaService;
        }

        // GET: api/AdMedia
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admedia>>> GetAdMedia()
        {
            var adMedia = await _adMadiaService.GetAllAdMediaAsync();
            return Ok(adMedia);
        }

        // GET: api/AdMedia/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Admedia>> GetAdMedia(int id)
        {
            var adMedia = await _adMadiaService.GetAdMediaByIdAsync(id);

            if (adMedia == null)
            {
                return NotFound();
            }

            return Ok(adMedia);
        }

        // POST: api/AdMedia
        [HttpPost]
        public async Task<ActionResult<Admedia>> CreateAdMedia(Admedia adMedia)
        {
            var createdAdMedia = await _adMadiaService.CreateAdMediaAsync(adMedia);
            return CreatedAtAction(nameof(GetAdMedia), new { id = createdAdMedia.MediaId }, createdAdMedia);
        }

        // PUT: api/AdMedia/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdMedia(int id, Admedia adMedia)
        {
            var result = await _adMadiaService.UpdateAdMediaAsync(id, adMedia);

            if (!result)
            {
                if (id != adMedia.MediaId)
                {
                    return BadRequest();
                }
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/AdMedia/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdMedia(int id)
        {
            var result = await _adMadiaService.DeleteAdMediaAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
