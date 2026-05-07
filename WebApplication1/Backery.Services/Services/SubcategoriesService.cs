using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    // Generyczna klasa wyników - powinna być osobno lub wspólna dla całej aplikacji
    public class SubcategoryService<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static SubcategoryService<T> GoodResult(
            string message,
            int statusCode,
            T? data = default)
            => new SubcategoryService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = true,
                Data = data
            };

        public static SubcategoryService<T> BadResult(
            string message,
            int statusCode,
            List<string>? errors = null,
            T? data = default)
            => new SubcategoryService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = false,
                Errors = errors ?? new List<string>(),
                Data = data
            };
    }

    // Właściwa implementacja serwisu podkategorii
    public class SubcategoriesService : ISubcategoriesService
    {
        private readonly AppDbContext _context;

        public SubcategoriesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SubcategoryService<IEnumerable<SubcategoryReadDto>>> GetAllSubcategoriesAsync()
        {
            try
            {
                var subcategories = await _context.Subcategories
                    .Include(sc => sc.Category)
                    .ToListAsync();

                var subcategoriesDtos = subcategories.Select(MapToSubcategoryReadDto);

                return SubcategoryService<IEnumerable<SubcategoryReadDto>>.GoodResult(
                    "Subcategories retrieved successfully",
                    200,
                    subcategoriesDtos);
            }
            catch (Exception ex)
            {
                return SubcategoryService<IEnumerable<SubcategoryReadDto>>.BadResult(
                    "Failed to retrieve subcategories",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<SubcategoryService<SubcategoryReadDto?>> GetSubcategoryAsync(int id)
        {
            try
            {
                var subcategory = await _context.Subcategories
                    .Include(sc => sc.Category)
                    .FirstOrDefaultAsync(sc => sc.SubcategoryId == id);

                if (subcategory == null)
                {
                    return SubcategoryService<SubcategoryReadDto?>.BadResult(
                        $"Subcategory with ID {id} not found",
                        404);
                }

                var subcategoryReadDto = MapToSubcategoryReadDto(subcategory);

                return SubcategoryService<SubcategoryReadDto?>.GoodResult(
                    "Subcategory retrieved successfully",
                    200,
                    subcategoryReadDto);
            }
            catch (Exception ex)
            {
                return SubcategoryService<SubcategoryReadDto?>.BadResult(
                    "Failed to retrieve subcategory",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<SubcategoryService<SubcategoryReadDto>> CreateSubcategoryAsync(SubcategoryCreateDto subcategoryCreateDto)
        {
            try
            {
                // Walidacja - sprawdź czy kategoria istnieje
                var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == subcategoryCreateDto.CategoryId);
                if (!categoryExists)
                {
                    return SubcategoryService<SubcategoryReadDto>.BadResult(
                        "Validation failed",
                        400,
                        new List<string> { $"Category with ID {subcategoryCreateDto.CategoryId} does not exist" });
                }

                // Sprawdź czy nazwa podkategorii jest unikalna w obrębie kategorii
                var nameExists = await _context.Subcategories
                    .AnyAsync(sc => sc.CategoryId == subcategoryCreateDto.CategoryId &&
                                   sc.Name.ToLower() == subcategoryCreateDto.Name.ToLower());
                if (nameExists)
                {
                    return SubcategoryService<SubcategoryReadDto>.BadResult(
                        "Validation failed",
                        400,
                        new List<string> { $"Subcategory with name '{subcategoryCreateDto.Name}' already exists in this category" });
                }

                var subcategory = new Subcategory
                {
                    CategoryId = subcategoryCreateDto.CategoryId,
                    Name = subcategoryCreateDto.Name.Trim()
                };

                _context.Subcategories.Add(subcategory);
                await _context.SaveChangesAsync();

                // Załaduj nawigację bez dodatkowego zapytania
                await _context.Entry(subcategory)
                    .Reference(sc => sc.Category)
                    .LoadAsync();

                var subcategoryReadDto = MapToSubcategoryReadDto(subcategory);

                return SubcategoryService<SubcategoryReadDto>.GoodResult(
                    "Subcategory created successfully",
                    201,
                    subcategoryReadDto);
            }
            catch (Exception ex)
            {
                return SubcategoryService<SubcategoryReadDto>.BadResult(
                    "Failed to create subcategory",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<SubcategoryService<SubcategoryReadDto>> UpdateSubcategoryAsync(int id, SubcategoryUpdateDto subcategoryUpdateDto)
        {
            try
            {
                if (id != subcategoryUpdateDto.SubcategoryId)
                {
                    return SubcategoryService<SubcategoryReadDto>.BadResult(
                        "Subcategory ID mismatch",
                        400,
                        new List<string> { "The provided ID does not match the subcategory ID in the request body" });
                }

                var existingSubcategory = await _context.Subcategories.FindAsync(id);
                if (existingSubcategory == null)
                {
                    return SubcategoryService<SubcategoryReadDto>.BadResult(
                        $"Subcategory with ID {id} not found",
                        404);
                }

                // Walidacja - sprawdź czy nowa kategoria istnieje
                var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == subcategoryUpdateDto.CategoryId);
                if (!categoryExists)
                {
                    return SubcategoryService<SubcategoryReadDto>.BadResult(
                        "Validation failed",
                        400,
                        new List<string> { $"Category with ID {subcategoryUpdateDto.CategoryId} does not exist" });
                }

                // Sprawdź czy nazwa jest unikalna w nowej kategorii (pomijając aktualny rekord)
                var nameExists = await _context.Subcategories
                    .AnyAsync(sc => sc.CategoryId == subcategoryUpdateDto.CategoryId &&
                                   sc.Name.ToLower() == subcategoryUpdateDto.Name.ToLower() &&
                                   sc.SubcategoryId != id);
                if (nameExists)
                {
                    return SubcategoryService<SubcategoryReadDto>.BadResult(
                        "Validation failed",
                        400,
                        new List<string> { $"Subcategory with name '{subcategoryUpdateDto.Name}' already exists in this category" });
                }

                // Aktualizacja właściwości
                existingSubcategory.CategoryId = subcategoryUpdateDto.CategoryId;
                existingSubcategory.Name = subcategoryUpdateDto.Name.Trim();

                _context.Entry(existingSubcategory).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Załaduj nawigację
                await _context.Entry(existingSubcategory)
                    .Reference(sc => sc.Category)
                    .LoadAsync();

                var subcategoryReadDto = MapToSubcategoryReadDto(existingSubcategory);

                return SubcategoryService<SubcategoryReadDto>.GoodResult(
                    "Subcategory updated successfully",
                    200,
                    subcategoryReadDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SubcategoryExistsAsync(id))
                {
                    return SubcategoryService<SubcategoryReadDto>.BadResult(
                        $"Subcategory with ID {id} not found",
                        404);
                }

                return SubcategoryService<SubcategoryReadDto>.BadResult(
                    "Concurrency conflict occurred while updating subcategory",
                    409);
            }
            catch (Exception ex)
            {
                return SubcategoryService<SubcategoryReadDto>.BadResult(
                    "Failed to update subcategory",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<SubcategoryService<bool>> DeleteSubcategoryAsync(int id)
        {
            try
            {
                var subcategory = await _context.Subcategories.FindAsync(id);
                if (subcategory == null)
                {
                    return SubcategoryService<bool>.BadResult(
                        $"Subcategory with ID {id} not found",
                        404,
                        data: false);
                }

                

                _context.Subcategories.Remove(subcategory);
                await _context.SaveChangesAsync();

                return SubcategoryService<bool>.GoodResult(
                    "Subcategory deleted successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return SubcategoryService<bool>.BadResult(
                    "Failed to delete subcategory",
                    500,
                    new List<string> { ex.Message },
                    false);
            }
        }

        public async Task<bool> SubcategoryExistsAsync(int id)
        {
            return await _context.Subcategories.AnyAsync(e => e.SubcategoryId == id);
        }

        // Prywatna metoda pomocnicza do mapowania
        private static SubcategoryReadDto MapToSubcategoryReadDto(Subcategory subcategory)
        {
            return new SubcategoryReadDto
            {
                SubcategoryId = subcategory.SubcategoryId,
                CategoryId = subcategory.CategoryId,
                Name = subcategory.Name,
                Category = subcategory.Category != null ? new CategoryBasicDto
                {
                    CategoryId = subcategory.Category.CategoryId,
                    Name = subcategory.Category.Name,
                } : null
            };
        }
    }
}
