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
    public class AdsController : ControllerBase
    {
        private readonly IAdsService _adsService;

        public AdsController(IAdsService adsService)
        {
            _adsService = adsService;
        }

        // GET: api/Ads
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ad>>> GetAds()
        {
            var ads = await _adsService.GetAllAdsAsync();
            return Ok(ads);
        }

        // GET: api/Ads/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ad>> GetAd(int id)
        {
            var ad = await _adsService.GetAdByIdAsync(id);

            if (ad == null)
            {
                return NotFound();
            }

            return Ok(ad);
        }

        // POST: api/Ads
        [HttpPost]
        public async Task<ActionResult<Ad>> CreateAd(Ad ad)
        {
            var createdAd = await _adsService.CreateAdAsync(ad);
            return CreatedAtAction(nameof(GetAd), new { id = createdAd.AdId }, createdAd);
        }

        // PUT: api/Ads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAd(int id, Ad ad)
        {
            var result = await _adsService.UpdateAdAsync(id, ad);

            if (!result)
            {
                if (id != ad.AdId)
                {
                    return BadRequest();
                }
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Ads/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAd(int id)
        {
            var result = await _adsService.DeleteAdAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
