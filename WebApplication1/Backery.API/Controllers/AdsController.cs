using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
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
        [Authorize(Roles = "Admin,User")]
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
        [Authorize(Roles = "Admin,User")]
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
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> CreateAd([FromBody] AdCreateDto dto)
        {
            var result = await _adsService.CreateAdAsync(dto);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
        [Authorize(Roles = "Admin,User")]
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
        [Authorize(Roles = "Admin")]
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