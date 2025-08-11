﻿using Microsoft.AspNetCore.Authorization;
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
    public class AdMediaController : ControllerBase
    {
        private readonly IAdMadiaService _adMadiaService;

        public AdMediaController(IAdMadiaService adMadiaService)
        {
            _adMadiaService = adMadiaService;
        }

        // GET: api/AdMedia
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AdMediaResponseDto>>> GetAdMedia()
        {
            var result = await _adMadiaService.GetAllAdMediaAsync();

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/AdMedia/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<AdMediaResponseDto>> GetAdMedia(int id)
        {
            var result = await _adMadiaService.GetAdMediaByIdAsync(id);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // POST: api/AdMedia
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AdMediaResponseDto>> CreateAdMedia(CreateAdMediaDto createAdMediaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid model state",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var result = await _adMadiaService.CreateAdMediaAsync(createAdMediaDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // PUT: api/AdMedia/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdateAdMedia(int id, UpdateAdMediaDto updateAdMediaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid model state",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            if (id != updateAdMediaDto.MediaId)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "ID w URL nie zgadza się z ID w obiekcie",
                    errors = new List<string> { "ID mismatch between URL and request body" }
                });
            }

            var result = await _adMadiaService.UpdateAdMediaAsync(id, updateAdMediaDto);

            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // DELETE: api/AdMedia/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteAdMedia(int id)
        {
            var result = await _adMadiaService.DeleteAdMediaAsync(id);

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