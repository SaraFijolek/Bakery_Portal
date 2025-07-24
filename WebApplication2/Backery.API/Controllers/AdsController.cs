using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication2.DTO;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdsController : ControllerBase
    {
        private readonly IAdsService _adsService;

        public AdsController(IAdsService adsService)
        {
            _adsService = adsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdResponseDto>>> GetAds()
        {
            var ads = await _adsService.GetAllAdsAsync();
            return Ok(ads);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdResponseDto>> GetAd(int id)
        {
            var ad = await _adsService.GetAdByIdAsync(id);
            if (ad == null) return NotFound();
            return Ok(ad);
        }

        [HttpPost]
        public async Task<ActionResult<AdResponseDto>> CreateAd([FromBody] AdCreateDto dto)
        {
            var created = await _adsService.CreateAdAsync(dto);
            return CreatedAtAction(nameof(GetAd), new { id = created.AdId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAd(int id, [FromBody] AdUpdateDto dto)
        {
            if (id != dto.AdId) return BadRequest();

            var ok = await _adsService.UpdateAdAsync(dto);
            if (!ok) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAd(int id)
        {
            var ok = await _adsService.DeleteAdAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
