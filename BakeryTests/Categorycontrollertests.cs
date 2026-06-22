using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Controllers.WebApplication2.Controllers;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;
using Xunit;

namespace WebApplication2.Tests.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _controller = new CategoriesController(_mockCategoryService.Object);
        }

        #region GetCategories

        [Fact]
        public async Task GetCategories_ReturnsOk_WhenCategoriesExist()
        {
            // Arrange
            var categories = new List<CategoryDto>
            {
                new CategoryDto { CategoryId = 1, Name = "Electronics" },
                new CategoryDto { CategoryId = 2, Name = "Books" }
            };

            _mockCategoryService
                .Setup(s => s.GetAllCategoriesAsync())
                .ReturnsAsync(ResultService<IEnumerable<CategoryDto>>.GoodResult(
                    "Categories retrieved successfully", 200, categories));

            // Act
            var result = await _controller.GetCategories();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, statusResult.StatusCode);
            Assert.Equal(categories, statusResult.Value);
        }

        [Fact]
        public async Task GetCategories_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockCategoryService
                .Setup(s => s.GetAllCategoriesAsync())
                .ReturnsAsync(ResultService<IEnumerable<CategoryDto>>.BadResult(
                    "Failed to retrieve categories", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.GetCategories();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetCategories_ReturnsOk_WhenListIsEmpty()
        {
            // Arrange
            var emptyList = new List<CategoryDto>();

            _mockCategoryService
                .Setup(s => s.GetAllCategoriesAsync())
                .ReturnsAsync(ResultService<IEnumerable<CategoryDto>>.GoodResult(
                    "Categories retrieved successfully", 200, emptyList));

            // Act
            var result = await _controller.GetCategories();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, statusResult.StatusCode);
        }

        #endregion

        #region GetCategory

        [Fact]
        public async Task GetCategory_ReturnsOk_WhenCategoryExists()
        {
            // Arrange
            var category = new CategoryDto { CategoryId = 1, Name = "Electronics" };

            _mockCategoryService
                .Setup(s => s.GetCategoryAsync(1))
                .ReturnsAsync(ResultService<CategoryDto>.GoodResult(
                    "Category retrieved successfully", 200, category));

            // Act
            var result = await _controller.GetCategory(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, statusResult.StatusCode);
            Assert.Equal(category, statusResult.Value);
        }

        [Fact]
        public async Task GetCategory_Returns404_WhenCategoryNotFound()
        {
            // Arrange
            _mockCategoryService
                .Setup(s => s.GetCategoryAsync(99))
                .ReturnsAsync(ResultService<CategoryDto>.BadResult(
                    "Category with ID 99 not found", 404));

            // Act
            var result = await _controller.GetCategory(99);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetCategory_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockCategoryService
                .Setup(s => s.GetCategoryAsync(1))
                .ReturnsAsync(ResultService<CategoryDto>.BadResult(
                    "Failed to retrieve category", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.GetCategory(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion

        #region CreateCategory

        [Fact]
        public async Task CreateCategory_Returns201_WhenCreatedSuccessfully()
        {
            // Arrange
            var createDto = new CreateCategoryDto { Name = "Electronics" };
            var createdCategory = new CategoryDto { CategoryId = 1, Name = "Electronics" };

            _mockCategoryService
                .Setup(s => s.CreateCategoryAsync(createDto))
                .ReturnsAsync(ResultService<CategoryDto>.GoodResult(
                    "Category created successfully", 201, createdCategory));

            // Act
            var result = await _controller.CreateCategory(createDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, statusResult.StatusCode);
            Assert.Equal(createdCategory, statusResult.Value);
        }

        [Fact]
        public async Task CreateCategory_Returns400_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = new CreateCategoryDto { Name = "" };
            _controller.ModelState.AddModelError("Name", "Category name is required");

            // Act
            var result = await _controller.CreateCategory(createDto);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateCategory_Returns500_WhenServiceFails()
        {
            // Arrange
            var createDto = new CreateCategoryDto { Name = "Electronics" };

            _mockCategoryService
                .Setup(s => s.CreateCategoryAsync(createDto))
                .ReturnsAsync(ResultService<CategoryDto>.BadResult(
                    "Failed to create category", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.CreateCategory(createDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateCategory_Returns400_WhenNameExceedsMaxLength()
        {
            // Arrange
            var createDto = new CreateCategoryDto { Name = new string('A', 101) };
            _controller.ModelState.AddModelError("Name", "Category name cannot exceed 100 characters");

            // Act
            var result = await _controller.CreateCategory(createDto);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, statusResult.StatusCode);
        }

        #endregion

        #region UpdateCategory

        [Fact]
        public async Task UpdateCategory_ReturnsOk_WhenUpdatedSuccessfully()
        {
            // Arrange
            var updateDto = new UpdateCategoryDto { Name = "Updated Electronics" };
            var updatedCategory = new CategoryDto { CategoryId = 1, Name = "Updated Electronics" };

            _mockCategoryService
                .Setup(s => s.UpdateCategoryAsync(1, updateDto))
                .ReturnsAsync(ResultService<CategoryDto>.GoodResult(
                    "Category updated successfully", 200, updatedCategory));

            // Act
            var result = await _controller.UpdateCategory(1, updateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, statusResult.StatusCode);
            Assert.Equal(updatedCategory, statusResult.Value);
        }

        [Fact]
        public async Task UpdateCategory_Returns404_WhenCategoryNotFound()
        {
            // Arrange
            var updateDto = new UpdateCategoryDto { Name = "Updated Electronics" };

            _mockCategoryService
                .Setup(s => s.UpdateCategoryAsync(99, updateDto))
                .ReturnsAsync(ResultService<CategoryDto>.BadResult(
                    "Category with ID 99 not found", 404));

            // Act
            var result = await _controller.UpdateCategory(99, updateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateCategory_Returns400_WhenModelStateIsInvalid()
        {
            // Arrange
            var updateDto = new UpdateCategoryDto { Name = "" };
            _controller.ModelState.AddModelError("Name", "Category name is required");

            // Act
            var result = await _controller.UpdateCategory(1, updateDto);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateCategory_Returns500_WhenServiceFails()
        {
            // Arrange
            var updateDto = new UpdateCategoryDto { Name = "Electronics" };

            _mockCategoryService
                .Setup(s => s.UpdateCategoryAsync(1, updateDto))
                .ReturnsAsync(ResultService<CategoryDto>.BadResult(
                    "Failed to update category", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.UpdateCategory(1, updateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion

        #region DeleteCategory

        [Fact]
        public async Task DeleteCategory_ReturnsOk_WhenDeletedSuccessfully()
        {
            // Arrange
            _mockCategoryService
                .Setup(s => s.DeleteCategoryAsync(1))
                .ReturnsAsync(ResultService<bool>.GoodResult(
                    "Category deleted successfully", 200, true));

            // Act
            var result = await _controller.DeleteCategory(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, statusResult.StatusCode);
        }

        [Fact]
        public async Task DeleteCategory_Returns404_WhenCategoryNotFound()
        {
            // Arrange
            _mockCategoryService
                .Setup(s => s.DeleteCategoryAsync(99))
                .ReturnsAsync(ResultService<bool>.BadResult(
                    "Category with ID 99 not found", 404, null, false));

            // Act
            var result = await _controller.DeleteCategory(99);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, statusResult.StatusCode);
        }

        [Fact]
        public async Task DeleteCategory_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockCategoryService
                .Setup(s => s.DeleteCategoryAsync(1))
                .ReturnsAsync(ResultService<bool>.BadResult(
                    "Failed to delete category", 500,
                    new List<string> { "Database error" }, false));

            // Act
            var result = await _controller.DeleteCategory(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion
    }
}