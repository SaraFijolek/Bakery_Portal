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
            public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }

            // GET: api/Categories/5
            [HttpGet("{id}")]
            public async Task<ActionResult<CategoryDto>> GetCategory(int id)
            {
                var category = await _categoryService.GetCategoryAsync(id);
                if (category == null)
                    return NotFound();

                return Ok(category);
            }

            // POST: api/Categories
            [HttpPost]
            public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdCategory = await _categoryService.CreateCategoryAsync(createCategoryDto);
                return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.CategoryId }, createdCategory);
            }

            // PUT: api/Categories/5
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                try
                {
                    var updatedCategory = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
                    return Ok(updatedCategory);
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

            // DELETE: api/Categories/5
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteCategory(int id)
            {
                var deleted = await _categoryService.DeleteCategoryAsync(id);
                if (!deleted)
                    return NotFound();

                return NoContent();
            }
        }
    }
}

