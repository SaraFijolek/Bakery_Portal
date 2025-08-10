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
        public async Task<ActionResult> GetAds()
        {
            var result = await _adsService.GetAllAdsAsync();

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAd(int id)
        {
            var result = await _adsService.GetAdByIdAsync(id);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        [HttpPost]
        public async Task<ActionResult> CreateAd([FromBody] AdCreateDto dto)
        {
            var result = await _adsService.CreateAdAsync(dto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAd(int id, [FromBody] AdUpdateDto dto)
        {
            if (id != dto.AdId)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "ID mismatch between route and request body",
                    errors = new List<string> { "The ID in the URL does not match the ID in the request body" }
                });
            }

            var result = await _adsService.UpdateAdAsync(dto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAd(int id)
        {
            var result = await _adsService.DeleteAdAsync(id);

            if (result.Success)
                return StatusCode(result.StatusCode, new
                {
                    success = true,
                    message = result.Message,
                    data = result.Data
                });

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }
    }
}