using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Controllers;
using WebApplication2.DTO;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;
using Xunit;

namespace WebApplication2.Tests.Controllers
{
    public class AdminPermissionsControllerTests
    {
        private readonly Mock<IAdminPermissionsService> _mockService;
        private readonly AdminPermissionsController _controller;

        public AdminPermissionsControllerTests()
        {
            _mockService = new Mock<IAdminPermissionsService>();
            _controller = new AdminPermissionsController(_mockService.Object);
        }

        // ───────────────────────────────────────────────
        // GET api/AdminPermissions
        // ───────────────────────────────────────────────

        [Fact]
        public async Task GetAdminPermissions_ReturnsOk_WhenPermissionsExist()
        {
            // Arrange
            var permissions = new List<AdminPermissionListItemDto>
            {
                new AdminPermissionListItemDto { PermissionId = 1, Name = "Read",  Category = "Content" },
                new AdminPermissionListItemDto { PermissionId = 2, Name = "Write", Category = "Content" }
            };

            _mockService
                .Setup(s => s.GetAllAdminPermissionsDtoAsync())
                .ReturnsAsync(ServiceResult<IEnumerable<AdminPermissionListItemDto>>.GoodResult(
                    "Uprawnienia zostały pobrane pomyślnie", 200, permissions));

            // Act
            var actionResult = await _controller.GetAdminPermissions();

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(permissions, result.Value);
        }

        [Fact]
        public async Task GetAdminPermissions_ReturnsOk_WhenListIsEmpty()
        {
            // Arrange
            var emptyList = new List<AdminPermissionListItemDto>();

            _mockService
                .Setup(s => s.GetAllAdminPermissionsDtoAsync())
                .ReturnsAsync(ServiceResult<IEnumerable<AdminPermissionListItemDto>>.GoodResult(
                    "Uprawnienia zostały pobrane pomyślnie", 200, emptyList));

            // Act
            var actionResult = await _controller.GetAdminPermissions();

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task GetAdminPermissions_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetAllAdminPermissionsDtoAsync())
                .ReturnsAsync(ServiceResult<IEnumerable<AdminPermissionListItemDto>>.BadResult(
                    "Błąd podczas pobierania uprawnień", 500,
                    new List<string> { "Database error" }));

            // Act
            var actionResult = await _controller.GetAdminPermissions();

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, result.StatusCode);
        }

        // ───────────────────────────────────────────────
        // GET api/AdminPermissions/{id}
        // ───────────────────────────────────────────────

        [Fact]
        public async Task GetAdminPermission_Returns200_WhenPermissionExists()
        {
            // Arrange
            var permission = new AdminPermissionResponseDto
            {
                PermissionId = 1,
                Name = "Read",
                Description = "Can read content",
                Category = "Content"
            };

            _mockService
                .Setup(s => s.GetAdminPermissionByIdDtoAsync(1))
                .ReturnsAsync(ServiceResult<AdminPermissionResponseDto>.GoodResult(
                    "Uprawnienie zostało pobrane pomyślnie", 200, permission));

            // Act
            var actionResult = await _controller.GetAdminPermission(1);

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(permission, result.Value);
        }

        [Fact]
        public async Task GetAdminPermission_Returns404_WhenPermissionNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetAdminPermissionByIdDtoAsync(99))
                .ReturnsAsync(ServiceResult<AdminPermissionResponseDto>.BadResult(
                    "Nie znaleziono uprawnienia o podanym ID", 404));

            // Act
            var actionResult = await _controller.GetAdminPermission(99);

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task GetAdminPermission_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetAdminPermissionByIdDtoAsync(1))
                .ReturnsAsync(ServiceResult<AdminPermissionResponseDto>.BadResult(
                    "Błąd podczas pobierania uprawnienia", 500,
                    new List<string> { "Database error" }));

            // Act
            var actionResult = await _controller.GetAdminPermission(1);

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, result.StatusCode);
        }

        // ───────────────────────────────────────────────
        // POST api/AdminPermissions
        // ───────────────────────────────────────────────

        [Fact]
        public async Task CreateAdminPermission_Returns201_WhenCreatedSuccessfully()
        {
            // Arrange
            var createDto = new CreateAdminPermissionDto
            {
                Name = "Delete",
                Description = "Can delete content",
                Category = "Content"
            };

            var responseDto = new AdminPermissionResponseDto
            {
                PermissionId = 3,
                Name = createDto.Name,
                Description = createDto.Description,
                Category = createDto.Category
            };

            _mockService
                .Setup(s => s.CreateAdminPermissionAsync(createDto))
                .ReturnsAsync(ServiceResult<AdminPermissionResponseDto>.GoodResult(
                    "Uprawnienie zostało utworzone pomyślnie", 201, responseDto));

            // Act
            var actionResult = await _controller.CreateAdminPermission(createDto);

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(201, result.StatusCode);

            var returned = Assert.IsType<AdminPermissionResponseDto>(result.Value);
            Assert.Equal(3, returned.PermissionId);
            Assert.Equal("Delete", returned.Name);
            Assert.Equal("Content", returned.Category);
        }

        [Fact]
        public async Task CreateAdminPermission_Returns500_WhenServiceFails()
        {
            // Arrange
            var createDto = new CreateAdminPermissionDto
            {
                Name = "Delete",
                Category = "Content"
            };

            _mockService
                .Setup(s => s.CreateAdminPermissionAsync(createDto))
                .ReturnsAsync(ServiceResult<AdminPermissionResponseDto>.BadResult(
                    "Błąd podczas tworzenia uprawnienia", 500,
                    new List<string> { "Database error" }));

            // Act
            var actionResult = await _controller.CreateAdminPermission(createDto);

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, result.StatusCode);
        }

        // ───────────────────────────────────────────────
        // PUT api/AdminPermissions/{id}
        // ───────────────────────────────────────────────

        [Fact]
        public async Task UpdateAdminPermission_Returns204_WhenUpdatedSuccessfully()
        {
            // Arrange
            var updateDto = new UpdateAdminPermissionDto
            {
                Name = "Read Updated",
                Description = "Updated description",
                Category = "Content"
            };

            _mockService
                .Setup(s => s.UpdateAdminPermissionAsync(1, updateDto))
                .ReturnsAsync(ServiceResult<object>.GoodResult(
                    "Uprawnienie zostało zaktualizowane pomyślnie", 204));

            // Act
            var result = await _controller.UpdateAdminPermission(1, updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(204, objectResult.StatusCode);
        }

        [Fact]
        public async Task UpdateAdminPermission_Returns404_WhenPermissionNotFound()
        {
            // Arrange
            var updateDto = new UpdateAdminPermissionDto
            {
                Name = "Non-existent",
                Category = "Content"
            };

            _mockService
                .Setup(s => s.UpdateAdminPermissionAsync(99, updateDto))
                .ReturnsAsync(ServiceResult<object>.BadResult(
                    "Nie znaleziono uprawnienia o podanym ID", 404));

            // Act
            var result = await _controller.UpdateAdminPermission(99, updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact]
        public async Task UpdateAdminPermission_Returns409_WhenConcurrencyConflict()
        {
            // Arrange
            var updateDto = new UpdateAdminPermissionDto
            {
                Name = "Read",
                Category = "Content"
            };

            _mockService
                .Setup(s => s.UpdateAdminPermissionAsync(1, updateDto))
                .ReturnsAsync(ServiceResult<object>.BadResult(
                    "Błąd współbieżności podczas aktualizacji uprawnienia", 409));

            // Act
            var result = await _controller.UpdateAdminPermission(1, updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, objectResult.StatusCode);
        }

        [Fact]
        public async Task UpdateAdminPermission_Returns500_WhenServiceFails()
        {
            // Arrange
            var updateDto = new UpdateAdminPermissionDto
            {
                Name = "Read",
                Category = "Content"
            };

            _mockService
                .Setup(s => s.UpdateAdminPermissionAsync(1, updateDto))
                .ReturnsAsync(ServiceResult<object>.BadResult(
                    "Błąd podczas aktualizacji uprawnienia", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.UpdateAdminPermission(1, updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        // ───────────────────────────────────────────────
        // DELETE api/AdminPermissions/{id}
        // ───────────────────────────────────────────────

        [Fact]
        public async Task DeleteAdminPermission_Returns204_WhenDeletedSuccessfully()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAdminPermissionAsync(1))
                .ReturnsAsync(ServiceResult<object>.GoodResult(
                    "Uprawnienie zostało usunięte pomyślnie", 204));

            // Act
            var result = await _controller.DeleteAdminPermission(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(204, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteAdminPermission_Returns404_WhenPermissionNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAdminPermissionAsync(99))
                .ReturnsAsync(ServiceResult<object>.BadResult(
                    "Nie znaleziono uprawnienia o podanym ID", 404));

            // Act
            var result = await _controller.DeleteAdminPermission(99);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteAdminPermission_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAdminPermissionAsync(1))
                .ReturnsAsync(ServiceResult<object>.BadResult(
                    "Błąd podczas usuwania uprawnienia", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.DeleteAdminPermission(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        // ───────────────────────────────────────────────
        // Weryfikacja wywołań serwisu (Verify)
        // ───────────────────────────────────────────────

        [Fact]
        public async Task GetAdminPermissions_CallsServiceExactlyOnce()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetAllAdminPermissionsDtoAsync())
                .ReturnsAsync(ServiceResult<IEnumerable<AdminPermissionListItemDto>>.GoodResult(
                    "OK", 200, new List<AdminPermissionListItemDto>()));

            // Act
            await _controller.GetAdminPermissions();

            // Assert
            _mockService.Verify(s => s.GetAllAdminPermissionsDtoAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAdminPermission_CallsServiceWithCorrectDto()
        {
            // Arrange
            var createDto = new CreateAdminPermissionDto { Name = "X", Category = "Y" };

            _mockService
                .Setup(s => s.CreateAdminPermissionAsync(createDto))
                .ReturnsAsync(ServiceResult<AdminPermissionResponseDto>.GoodResult(
                    "OK", 201, new AdminPermissionResponseDto()));

            // Act
            await _controller.CreateAdminPermission(createDto);

            // Assert
            _mockService.Verify(s => s.CreateAdminPermissionAsync(createDto), Times.Once);
        }

        [Fact]
        public async Task DeleteAdminPermission_CallsServiceWithCorrectId()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAdminPermissionAsync(5))
                .ReturnsAsync(ServiceResult<object>.GoodResult("OK", 204));

            // Act
            await _controller.DeleteAdminPermission(5);

            // Assert
            _mockService.Verify(s => s.DeleteAdminPermissionAsync(5), Times.Once);
        }
    }
}