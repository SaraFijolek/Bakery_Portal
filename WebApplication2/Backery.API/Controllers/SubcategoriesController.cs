using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubcategoriesController : ControllerBase
    {
        private readonly ISubcategoriesService _subcategoriesService;

        public SubcategoriesController(ISubcategoriesService subcategoriesService)
        {
            _subcategoriesService = subcategoriesService;
        }

        // GET: api/Subcategories
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SubcategoryReadDto>>> GetSubcategories()
        {
            var result = await _subcategoriesService.GetAllSubcategoriesAsync();
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // GET: api/Subcategories/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<SubcategoryReadDto>> GetSubcategory(int id)
        {
            var result = await _subcategoriesService.GetSubcategoryAsync(id);
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // POST: api/Subcategories
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SubcategoryReadDto>> CreateSubcategory(SubcategoryCreateDto subcategoryCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = ModelState.Values
                                 .SelectMany(v => v.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList()
                });

            var result = await _subcategoriesService.CreateSubcategoryAsync(subcategoryCreateDto);
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // PUT: api/Subcategories/5
        [HttpPut("{id}")]
        [Authorize(Roles ="Admin,User")]
        public async Task<IActionResult> UpdateSubcategory(int id, SubcategoryUpdateDto subcategoryUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage)
                                .ToList()
                });


            var result = await _subcategoriesService.UpdateSubcategoryAsync(id, subcategoryUpdateDto);
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });

        }
        

        // DELETE: api/Subcategories/5
        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin")]

        public async Task<IActionResult> DeleteSubcategory(int id)
        {
            var result = await _subcategoriesService.DeleteSubcategoryAsync(id);
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
