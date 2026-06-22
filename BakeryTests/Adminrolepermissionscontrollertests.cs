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
    public class AdminRolePermissionsControllerTests
    {
        private readonly Mock<IAdminRolePermissionsService> _mockService;
        private readonly AdminRolePermissionsController _controller;

        public AdminRolePermissionsControllerTests()
        {
            _mockService = new Mock<IAdminRolePermissionsService>();
            _controller = new AdminRolePermissionsController(_mockService.Object);
        }

        // ───────────────────────────────────────────────
        // Helpers
        // ───────────────────────────────────────────────

        private static AdminRolePermissionListItemDto MakeListItem(int id = 1) =>
            new AdminRolePermissionListItemDto
            {
                RolePermissionId = id,
                Role = "Admin",
                PermissionId = 10,
                PermissionName = "Read",
                PermissionCategory = "Content"
            };

        private static AdminRolePermissionResponseDto MakeResponseDto(int id = 1) =>
            new AdminRolePermissionResponseDto
            {
                RolePermissionId = id,
                Role = "Admin",
                PermissionId = 10,
                AdminPermission = new AdminPermissionDto
                {
                    PermissionId = 10,
                    Name = "Read",
                    Description = "Can read content",
                    Category = "Content"
                }
            };

        // ───────────────────────────────────────────────
        // GET api/AdminRolePermissions
        // ───────────────────────────────────────────────

        [Fact]
        public async Task GetAdminRolePermissions_Returns200_WithList()
        {
            // Arrange
            var items = new List<AdminRolePermissionListItemDto>
            {
                MakeListItem(1),
                MakeListItem(2)
            };

            _mockService
                .Setup(s => s.GetAllAdminRolePermissionsDtoAsync())
                .ReturnsAsync(ResultService<IEnumerable<AdminRolePermissionListItemDto>>.GoodResult(
                    "Admin role permissions retrieved successfully", 200, items));

            // Act
            var actionResult = await _controller.GetAdminRolePermissions();

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(items, result.Value);
        }

        [Fact]
        public async Task GetAdminRolePermissions_Returns200_WithEmptyList()
        {
            // Arrange
            var emptyList = new List<AdminRolePermissionListItemDto>();

            _mockService
                .Setup(s => s.GetAllAdminRolePermissionsDtoAsync())
                .ReturnsAsync(ResultService<IEnumerable<AdminRolePermissionListItemDto>>.GoodResult(
                    "Admin role permissions retrieved successfully", 200, emptyList));

            // Act
            var actionResult = await _controller.GetAdminRolePermissions();

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task GetAdminRolePermissions_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetAllAdminRolePermissionsDtoAsync())
                .ReturnsAsync(ResultService<IEnumerable<AdminRolePermissionListItemDto>>.BadResult(
                    "Error occurred while retrieving admin role permissions", 500,
                    new List<string> { "Database connection failed" }));

            // Act
            var actionResult = await _controller.GetAdminRolePermissions();

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task GetAdminRolePermissions_CallsServiceExactlyOnce()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetAllAdminRolePermissionsDtoAsync())
                .ReturnsAsync(ResultService<IEnumerable<AdminRolePermissionListItemDto>>.GoodResult(
                    "OK", 200, new List<AdminRolePermissionListItemDto>()));

            // Act
            await _controller.GetAdminRolePermissions();

            // Assert
            _mockService.Verify(s => s.GetAllAdminRolePermissionsDtoAsync(), Times.Once);
        }

        // ───────────────────────────────────────────────
        // GET api/AdminRolePermissions/{id}
        // ───────────────────────────────────────────────

        [Fact]
        public async Task GetAdminRolePermission_Returns200_WhenFound()
        {
            // Arrange
            var dto = MakeResponseDto(1);

            _mockService
                .Setup(s => s.GetAdminRolePermissionByIdDtoAsync(1))
                .ReturnsAsync(ResultService<AdminRolePermissionResponseDto>.GoodResult(
                    "Admin role permission retrieved successfully", 200, dto));

            // Act
            var actionResult = await _controller.GetAdminRolePermission(1);

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(dto, result.Value);
        }

        [Fact]
        public async Task GetAdminRolePermission_Returns404_WhenNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetAdminRolePermissionByIdDtoAsync(99))
                .ReturnsAsync(ResultService<AdminRolePermissionResponseDto>.BadResult(
                    "Admin role permission with ID 99 not found", 404));

            // Act
            var actionResult = await _controller.GetAdminRolePermission(99);

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task GetAdminRolePermission_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetAdminRolePermissionByIdDtoAsync(1))
                .ReturnsAsync(ResultService<AdminRolePermissionResponseDto>.BadResult(
                    "Error occurred while retrieving admin role permission", 500,
                    new List<string> { "Database error" }));

            // Act
            var actionResult = await _controller.GetAdminRolePermission(1);

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task GetAdminRolePermission_ReturnsCorrectPermissionData()
        {
            // Arrange
            var dto = MakeResponseDto(5);

            _mockService
                .Setup(s => s.GetAdminRolePermissionByIdDtoAsync(5))
                .ReturnsAsync(ResultService<AdminRolePermissionResponseDto>.GoodResult(
                    "OK", 200, dto));

            // Act
            var actionResult = await _controller.GetAdminRolePermission(5);

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            var returned = Assert.IsType<AdminRolePermissionResponseDto>(result.Value);
            Assert.Equal(5, returned.RolePermissionId);
            Assert.Equal("Admin", returned.Role);
            Assert.Equal(10, returned.PermissionId);
            Assert.NotNull(returned.AdminPermission);
            Assert.Equal("Read", returned.AdminPermission.Name);
        }

        // ───────────────────────────────────────────────
        // POST api/AdminRolePermissions
        // ───────────────────────────────────────────────

        [Fact]
        public async Task CreateAdminRolePermission_Returns201_WhenCreatedSuccessfully()
        {
            // Arrange
            var createDto = new CreateAdminRolePermissionDto { Role = "Admin", PermissionId = 10 };
            var responseDto = MakeResponseDto(3);

            _mockService
                .Setup(s => s.CreateAdminRolePermissionAsync(createDto))
                .ReturnsAsync(ResultService<AdminRolePermissionResponseDto>.GoodResult(
                    "Admin role permission created successfully", 201, responseDto));

            // Act
            var actionResult = await _controller.CreateAdminRolePermission(createDto);

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(201, result.StatusCode);
        }

        [Fact]
        public async Task CreateAdminRolePermission_ReturnsCreatedDto()
        {
            // Arrange
            var createDto = new CreateAdminRolePermissionDto { Role = "Moderator", PermissionId = 5 };
            var responseDto = new AdminRolePermissionResponseDto
            {
                RolePermissionId = 7,
                Role = "Moderator",
                PermissionId = 5,
                AdminPermission = new AdminPermissionDto
                {
                    PermissionId = 5,
                    Name = "Write",
                    Description = "Can write content",
                    Category = "Content"
                }
            };

            _mockService
                .Setup(s => s.CreateAdminRolePermissionAsync(createDto))
                .ReturnsAsync(ResultService<AdminRolePermissionResponseDto>.GoodResult(
                    "Admin role permission created successfully", 201, responseDto));

            // Act
            var actionResult = await _controller.CreateAdminRolePermission(createDto);

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            var returned = Assert.IsType<AdminRolePermissionResponseDto>(result.Value);
            Assert.Equal(7, returned.RolePermissionId);
            Assert.Equal("Moderator", returned.Role);
            Assert.Equal("Write", returned.AdminPermission.Name);
        }

        [Fact]
        public async Task CreateAdminRolePermission_Returns500_WhenServiceFails()
        {
            // Arrange
            var createDto = new CreateAdminRolePermissionDto { Role = "Admin", PermissionId = 10 };

            _mockService
                .Setup(s => s.CreateAdminRolePermissionAsync(createDto))
                .ReturnsAsync(ResultService<AdminRolePermissionResponseDto>.BadResult(
                    "Error occurred while creating admin role permission", 500,
                    new List<string> { "Database error" }));

            // Act
            var actionResult = await _controller.CreateAdminRolePermission(createDto);

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task CreateAdminRolePermission_CallsServiceWithCorrectDto()
        {
            // Arrange
            var createDto = new CreateAdminRolePermissionDto { Role = "Admin", PermissionId = 10 };

            _mockService
                .Setup(s => s.CreateAdminRolePermissionAsync(createDto))
                .ReturnsAsync(ResultService<AdminRolePermissionResponseDto>.GoodResult(
                    "OK", 201, MakeResponseDto()));

            // Act
            await _controller.CreateAdminRolePermission(createDto);

            // Assert
            _mockService.Verify(s => s.CreateAdminRolePermissionAsync(createDto), Times.Once);
        }

        // ───────────────────────────────────────────────
        // DELETE api/AdminRolePermissions/{id}
        // ───────────────────────────────────────────────

        [Fact]
        public async Task DeleteAdminRolePermission_Returns200_WhenDeletedSuccessfully()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAdminRolePermissionAsync(1))
                .ReturnsAsync(ResultService<bool>.GoodResult(
                    "Admin role permission deleted successfully", 200, true));

            // Act
            var result = await _controller.DeleteAdminRolePermission(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(true, objectResult.Value);
        }

        [Fact]
        public async Task DeleteAdminRolePermission_Returns404_WhenNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAdminRolePermissionAsync(99))
                .ReturnsAsync(ResultService<bool>.BadResult(
                    "Admin role permission with ID 99 not found", 404));

            // Act
            var result = await _controller.DeleteAdminRolePermission(99);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteAdminRolePermission_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAdminRolePermissionAsync(1))
                .ReturnsAsync(ResultService<bool>.BadResult(
                    "Error occurred while deleting admin role permission", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.DeleteAdminRolePermission(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteAdminRolePermission_CallsServiceWithCorrectId()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAdminRolePermissionAsync(42))
                .ReturnsAsync(ResultService<bool>.GoodResult("OK", 200, true));

            // Act
            await _controller.DeleteAdminRolePermission(42);

            // Assert
            _mockService.Verify(s => s.DeleteAdminRolePermissionAsync(42), Times.Once);
        }

        [Fact]
        public async Task DeleteAdminRolePermission_DoesNotCallDelete_ForDifferentId()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAdminRolePermissionAsync(It.IsAny<int>()))
                .ReturnsAsync(ResultService<bool>.GoodResult("OK", 200, true));

            // Act
            await _controller.DeleteAdminRolePermission(1);

            // Assert
            _mockService.Verify(s => s.DeleteAdminRolePermissionAsync(2), Times.Never);
        }
    }
}