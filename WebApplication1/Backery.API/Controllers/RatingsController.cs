using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingsService _ratingsService;

        public RatingsController(IRatingsService ratingsService)
        {
            _ratingsService = ratingsService;
        }

        // GET: api/Ratings
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetRatings()
        {
            var result = await _ratingsService.GetRatingsAsync();

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/Ratings/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetRating(int id)
        {
            var result = await _ratingsService.GetRatingByIdAsync(id);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // POST: api/Ratings
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> CreateRating(CreateRatingDto createRatingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Model validation failed",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });

            var result = await _ratingsService.CreateRatingAsync(createRatingDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // PUT: api/Ratings/5
        [HttpPut("{id}")]
        [Authorize(Roles =  "Admin")]
        public async Task<ActionResult> UpdateRating(int id, UpdateRatingDto updateRatingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Model validation failed",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });

            var result = await _ratingsService.UpdateRatingAsync(id, updateRatingDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // DELETE: api/Ratings/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteRating(int id)
        {
            var result = await _ratingsService.DeleteRatingAsync(id);

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