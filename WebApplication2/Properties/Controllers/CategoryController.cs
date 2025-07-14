using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Properties.Data;
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
            public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }

            // GET: api/Categories/5
            [HttpGet("{id}")]
            public async Task<ActionResult<Category>> GetCategory(int id)
            {
                var category = await _categoryService.GetCategoryAsync(id);
                if (category == null)
                    return NotFound();

                return Ok(category);
            }

            // POST: api/Categories
            [HttpPost]
            public async Task<ActionResult<Category>> CreateCategory(Category category)
            {
                var createdCategory = await _categoryService.CreateCategoryAsync(category);
                return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.CategoryId }, createdCategory);
            }

            // PUT: api/Categories/5
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateCategory(int id, Category category)
            {
                try
                {
                    await _categoryService.UpdateCategoryAsync(id, category);
                    return NoContent();
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

