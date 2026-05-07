using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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


    namespace WebApplication2.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class CategoriesController : ControllerBase
        {
            private readonly ICategoryService _categoryService;

            public CategoriesController(ICategoryService categoryService)
            {
                _categoryService = categoryService;
            }

            // GET: api/Categories
            [HttpGet]
            [AllowAnonymous]
            public async Task<ActionResult> GetCategories()
            {
                var result = await _categoryService.GetAllCategoriesAsync();

                if (result.Success)
                    return StatusCode(result.StatusCode, result.Data);

                return StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
            }

            // GET: api/Categories/5
            [HttpGet("{id}")]
            [AllowAnonymous]
            public async Task<ActionResult> GetCategory(int id)
            {
                var result = await _categoryService.GetCategoryAsync(id);

                if (result.Success)
                    return StatusCode(result.StatusCode, result.Data);

                return StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
            }

            // POST: api/Categories
            [HttpPost]
            [Authorize]
            public async Task<ActionResult> CreateCategory(CreateCategoryDto createCategoryDto)
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

                var result = await _categoryService.CreateCategoryAsync(createCategoryDto);

                if (result.Success)
                    return StatusCode(result.StatusCode, result.Data);

                return StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
            }

            // PUT: api/Categories/5
            [HttpPut("{id}")]
            [Authorize(Roles = "Admin,User")]
            public async Task<ActionResult> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
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

                var result = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);

                if (result.Success)
                    return StatusCode(result.StatusCode, result.Data);

                return StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
            }

            // DELETE: api/Categories/5
            [HttpDelete("{id}")]
            [Authorize(Roles ="Admin")]
            public async Task<ActionResult> DeleteCategory(int id)
            {
                var result = await _categoryService.DeleteCategoryAsync(id);

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
}

