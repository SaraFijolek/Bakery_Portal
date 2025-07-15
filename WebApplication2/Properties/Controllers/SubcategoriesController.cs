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
        public async Task<ActionResult<IEnumerable<Subcategory>>> GetSubcategories()
        {
            var subcategories = await _subcategoriesService.GetAllSubcategoriesAsync();
            return Ok(subcategories);
        }

        // GET: api/Subcategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subcategory>> GetSubcategory(int id)
        {
            var subcategory = await _subcategoriesService.GetSubcategoryAsync(id);
            if (subcategory == null)
                return NotFound();

            return Ok(subcategory);
        }

        // POST: api/Subcategories
        [HttpPost]
        public async Task<ActionResult<Subcategory>> CreateSubcategory(Subcategory subcategory)
        {
            var createdSubcategory = await _subcategoriesService.CreateSubcategoryAsync(subcategory);
            return CreatedAtAction(nameof(GetSubcategory), new { id = createdSubcategory.SubcategoryId }, createdSubcategory);
        }

        // PUT: api/Subcategories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubcategory(int id, Subcategory subcategory)
        {
            try
            {
                await _subcategoriesService.UpdateSubcategoryAsync(id, subcategory);
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

        // DELETE: api/Subcategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubcategory(int id)
        {
            var deleted = await _subcategoriesService.DeleteSubcategoryAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
