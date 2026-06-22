using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Controllers;
using WebApplication2.DTO;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;
using Xunit;

namespace WebApplication2.Tests.Controllers
{
    public class SubcategoriesControllerTests
    {
        private readonly Mock<ISubcategoriesService> _serviceMock;
        private readonly SubcategoriesController _controller;

        public SubcategoriesControllerTests()
        {
            _serviceMock = new Mock<ISubcategoriesService>();
            _controller = new SubcategoriesController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetSubcategories_ReturnsOk_WhenServiceSucceeds()
        {
            // Arrange
            var data = new List<SubcategoryReadDto>
            {
                new()
                {
                    SubcategoryId = 1,
                    CategoryId = 1,
                    Name = "Laptops"
                }
            };

            _serviceMock.Setup(s => s.GetAllSubcategoriesAsync())
                .ReturnsAsync(SubcategoryService<IEnumerable<SubcategoryReadDto>>
                    .GoodResult("Success", 200, data));

            // Act
            var result = await _controller.GetSubcategories();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, objectResult.StatusCode);

            var returnedData =
                Assert.IsAssignableFrom<IEnumerable<SubcategoryReadDto>>(objectResult.Value);

            Assert.Single(returnedData);
        }

        [Fact]
        public async Task GetSubcategory_ReturnsNotFound_WhenSubcategoryDoesNotExist()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetSubcategoryAsync(1))
                .ReturnsAsync(SubcategoryService<SubcategoryReadDto?>
                    .BadResult("Not found", 404));

            // Act
            var result = await _controller.GetSubcategory(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);

            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateSubcategory_ReturnsCreated_WhenValid()
        {
            // Arrange
            var createDto = new SubcategoryCreateDto
            {
                CategoryId = 1,
                Name = "Gaming"
            };

            var readDto = new SubcategoryReadDto
            {
                SubcategoryId = 10,
                CategoryId = 1,
                Name = "Gaming"
            };

            _serviceMock.Setup(s => s.CreateSubcategoryAsync(createDto))
                .ReturnsAsync(SubcategoryService<SubcategoryReadDto>
                    .GoodResult("Created", 201, readDto));

            // Act
            var result = await _controller.CreateSubcategory(createDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);

            Assert.Equal(201, objectResult.StatusCode);

            var returnedDto =
                Assert.IsType<SubcategoryReadDto>(objectResult.Value);

            Assert.Equal("Gaming", returnedDto.Name);
        }

        [Fact]
        public async Task CreateSubcategory_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Name is required");

            var dto = new SubcategoryCreateDto();

            // Act
            var result = await _controller.CreateSubcategory(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task UpdateSubcategory_ReturnsOk_WhenUpdated()
        {
            // Arrange
            var dto = new SubcategoryUpdateDto
            {
                CategoryId = 1,
                Name = "Updated Name"
            };

            var updatedDto = new SubcategoryReadDto
            {
                SubcategoryId = 1,
                CategoryId = 1,
                Name = "Updated Name"
            };

            _serviceMock.Setup(s => s.UpdateSubcategoryAsync(1, dto))
                .ReturnsAsync(SubcategoryService<SubcategoryReadDto>
                    .GoodResult("Updated", 200, updatedDto));

            // Act
            var result = await _controller.UpdateSubcategory(1, dto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteSubcategory_ReturnsOk_WhenDeleted()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteSubcategoryAsync(1))
                .ReturnsAsync(SubcategoryService<bool>
                    .GoodResult("Deleted", 200, true));

            // Act
            var result = await _controller.DeleteSubcategory(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteSubcategory_ReturnsNotFound_WhenSubcategoryDoesNotExist()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteSubcategoryAsync(999))
                .ReturnsAsync(SubcategoryService<bool>
                    .BadResult("Not found", 404));

            // Act
            var result = await _controller.DeleteSubcategory(999);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(404, objectResult.StatusCode);
        }
    }
}