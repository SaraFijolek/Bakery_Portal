﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;
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
        public async Task<ActionResult<IEnumerable<RatingDto>>> GetRatings()
        {
            var ratings = await _ratingsService.GetRatingsAsync();
            return Ok(ratings);
        }

        // GET: api/Ratings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RatingDto>> GetRating(int id)
        {
            var rating = await _ratingsService.GetRatingByIdAsync(id);
            if (rating == null)
                return NotFound();
            return Ok(rating);
        }

        // POST: api/Ratings
        [HttpPost]
        public async Task<ActionResult<RatingDto>> CreateRating(CreateRatingDto createRatingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdRating = await _ratingsService.CreateRatingAsync(createRatingDto);
            return CreatedAtAction(nameof(GetRating), new { id = createdRating.RatingId }, createdRating);
        }

        // PUT: api/Ratings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRating(int id, UpdateRatingDto updateRatingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedRating = await _ratingsService.UpdateRatingAsync(id, updateRatingDto);
                return Ok(updatedRating);
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

        // DELETE: api/Ratings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var deleted = await _ratingsService.DeleteRatingAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
