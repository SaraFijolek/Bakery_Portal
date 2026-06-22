using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Controllers;
using WebApplication2.DTO;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;
using Xunit;

namespace WebApplication2.Tests.Controllers
{
    public class AdminsControllerTests
    {
        private readonly Mock<IAdminsService> _mockService;
        private readonly AdminsController _controller;

        public AdminsControllerTests()
        {
            _mockService = new Mock<IAdminsService>();
            _controller = new AdminsController(_mockService.Object);
        }

        // ───────────────────────────────────────────────
        // Helpers
        // ───────────────────────────────────────────────

        private static AdminDto MakeAdminDto(string id = "admin-1") => new AdminDto
        {
            AdminId = id,
            Email = "admin@test.com",
            Name = "Test Admin",
            Phone = "123456789",
            Role = "Admin",
            IsActive = true,
            TwoFactorEnabled = false,
            CreatedAt = new DateTime(2024, 1, 1)
        };

        private static AdminListDto MakeAdminListDto(string id = "admin-1") => new AdminListDto
        {
            AdminId = id,
            Email = "admin@test.com",
            Name = "Test Admin",
            Role = "Admin",
            IsActive = true,
            TwoFactorEnabled = false,
            CreatedAt = new DateTime(2024, 1, 1),
            SessionsCount = 2,
            AuditLogsCount = 5,
            UserModerationsCount = 1,
            AdModerationsCount = 0,
            ReportedContentsCount = 3
        };

        private static CreateAdminDto MakeCreateDto() => new CreateAdminDto
        {
            Email = "new@test.com",
            PasswordHash = "hashedpassword123",
            Name = "New Admin",
            Role = "moderator",
            IsActive = true
        };

        private static UpdateAdminDto MakeUpdateDto() => new UpdateAdminDto
        {
            Email = "updated@test.com",
            Name = "Updated Admin",
            Role = "Admin",
            IsActive = true,
            TwoFactorEnabled = false
        };

        // ───────────────────────────────────────────────
        // GET api/Admins
        // ───────────────────────────────────────────────

        [Fact]
        public async Task GetAdmins_Returns200_WithAdminList()
        {
            // Arrange
            var admins = new List<AdminDto> { MakeAdminDto("1"), MakeAdminDto("2") };

            _mockService
                .Setup(s => s.GetAllAdminsAsync())
                .ReturnsAsync(AdminService<IEnumerable<AdminDto>>.GoodResult(
                    "Admins retrieved successfully", 200, admins));

            // Act
            var result = await _controller.GetAdmins();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(admins, objectResult.Value);
        }

        [Fact]
        public async Task GetAdmins_Returns200_WithEmptyList()
        {
            // Arrange
            var emptyList = new List<AdminDto>();

            _mockService
                .Setup(s => s.GetAllAdminsAsync())
                .ReturnsAsync(AdminService<IEnumerable<AdminDto>>.GoodResult(
                    "Admins retrieved successfully", 200, emptyList));

            // Act
            var result = await _controller.GetAdmins();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetAdmins_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetAllAdminsAsync())
                .ReturnsAsync(AdminService<IEnumerable<AdminDto>>.BadResult(
                    "Failed to retrieve admins", 500,
                    new List<string> { "Database connection failed" }));

            // Act
            var result = await _controller.GetAdmins();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetAdmins_CallsServiceExactlyOnce()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetAllAdminsAsync())
                .ReturnsAsync(AdminService<IEnumerable<AdminDto>>.GoodResult(
                    "OK", 200, new List<AdminDto>()));

            // Act
            await _controller.GetAdmins();

            // Assert
            _mockService.Verify(s => s.GetAllAdminsAsync(), Times.Once);
        }

        // ───────────────────────────────────────────────
        // GET api/Admins/{id}
        // ───────────────────────────────────────────────

        [Fact]
        public async Task GetAdmin_Returns200_WhenAdminFound()
        {
            // Arrange
            var dto = MakeAdminDto("admin-1");

            _mockService
                .Setup(s => s.GetAdminByIdAsync("admin-1"))
                .ReturnsAsync(AdminService<AdminDto>.GoodResult(
                    "Admin retrieved successfully", 200, dto));

            // Act
            var result = await _controller.GetAdmin("admin-1");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(dto, objectResult.Value);
        }

        [Fact]
        public async Task GetAdmin_Returns404_WhenAdminNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetAdminByIdAsync("nonexistent"))
                .ReturnsAsync(AdminService<AdminDto>.BadResult(
                    "Admin with ID nonexistent not found", 404));

            // Act
            var result = await _controller.GetAdmin("nonexistent");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetAdmin_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetAdminByIdAsync("admin-1"))
                .ReturnsAsync(AdminService<AdminDto>.BadResult(
                    "Failed to retrieve admin", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.GetAdmin("admin-1");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetAdmin_ReturnsCorrectAdminData()
        {
            // Arrange
            var dto = new AdminDto
            {
                AdminId = "admin-42",
                Email = "specific@test.com",
                Name = "Specific Admin",
                Role = "Admin",
                IsActive = true,
                TwoFactorEnabled = true,
                CreatedAt = new DateTime(2023, 6, 15)
            };

            _mockService
                .Setup(s => s.GetAdminByIdAsync("admin-42"))
                .ReturnsAsync(AdminService<AdminDto>.GoodResult("OK", 200, dto));

            // Act
            var result = await _controller.GetAdmin("admin-42");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var returned = Assert.IsType<AdminDto>(objectResult.Value);
            Assert.Equal("admin-42", returned.AdminId);
            Assert.Equal("specific@test.com", returned.Email);
            Assert.True(returned.TwoFactorEnabled);
        }

        [Fact]
        public async Task GetAdmin_CallsServiceWithCorrectId()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetAdminByIdAsync("admin-7"))
                .ReturnsAsync(AdminService<AdminDto>.GoodResult("OK", 200, MakeAdminDto("admin-7")));

            // Act
            await _controller.GetAdmin("admin-7");

            // Assert
            _mockService.Verify(s => s.GetAdminByIdAsync("admin-7"), Times.Once);
        }

        // ───────────────────────────────────────────────
        // POST api/Admins
        // ───────────────────────────────────────────────

        [Fact]
        public async Task CreateAdmin_Returns201_WhenCreatedSuccessfully()
        {
            // Arrange
            var createDto = MakeCreateDto();
            var responseDto = MakeAdminDto("new-admin-1");

            _mockService
                .Setup(s => s.CreateAdminAsync(createDto))
                .ReturnsAsync(AdminService<AdminDto>.GoodResult(
                    "Admin created successfully", 201, responseDto));

            // Act
            var result = await _controller.CreateAdmin(createDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode);
            Assert.Equal(responseDto, objectResult.Value);
        }

        [Fact]
        public async Task CreateAdmin_Returns400_WhenValidationFails()
        {
            // Arrange
            var createDto = new CreateAdminDto
            {
                Email = "",   // brak emaila — błąd walidacji
                PasswordHash = "hash",
                Name = "Admin",
                Role = "moderator"
            };

            _mockService
                .Setup(s => s.CreateAdminAsync(createDto))
                .ReturnsAsync(AdminService<AdminDto>.BadResult(
                    "Validation failed", 400,
                    new List<string> { "Email is required" }));

            // Act
            var result = await _controller.CreateAdmin(createDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateAdmin_Returns400_WhenEmailAlreadyExists()
        {
            // Arrange
            var createDto = MakeCreateDto();

            _mockService
                .Setup(s => s.CreateAdminAsync(createDto))
                .ReturnsAsync(AdminService<AdminDto>.BadResult(
                    "Validation failed", 400,
                    new List<string> { "Admin with this email already exists" }));

            // Act
            var result = await _controller.CreateAdmin(createDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateAdmin_Returns500_WhenServiceFails()
        {
            // Arrange
            var createDto = MakeCreateDto();

            _mockService
                .Setup(s => s.CreateAdminAsync(createDto))
                .ReturnsAsync(AdminService<AdminDto>.BadResult(
                    "Failed to create admin", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.CreateAdmin(createDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateAdmin_CallsServiceWithCorrectDto()
        {
            // Arrange
            var createDto = MakeCreateDto();

            _mockService
                .Setup(s => s.CreateAdminAsync(createDto))
                .ReturnsAsync(AdminService<AdminDto>.GoodResult("OK", 201, MakeAdminDto()));

            // Act
            await _controller.CreateAdmin(createDto);

            // Assert
            _mockService.Verify(s => s.CreateAdminAsync(createDto), Times.Once);
        }

        // ───────────────────────────────────────────────
        // PUT api/Admins/{id}
        // ───────────────────────────────────────────────

        [Fact]
        public async Task UpdateAdmin_Returns200_WhenUpdatedSuccessfully()
        {
            // Arrange
            var updateDto = MakeUpdateDto();
            var responseDto = MakeAdminDto("admin-1");

            _mockService
                .Setup(s => s.UpdateAdminAsync("admin-1", updateDto))
                .ReturnsAsync(AdminService<AdminDto>.GoodResult(
                    "Admin updated successfully", 200, responseDto));

            // Act
            var result = await _controller.UpdateAdmin("admin-1", updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(responseDto, objectResult.Value);
        }

        [Fact]
        public async Task UpdateAdmin_Returns404_WhenAdminNotFound()
        {
            // Arrange
            var updateDto = MakeUpdateDto();

            _mockService
                .Setup(s => s.UpdateAdminAsync("nonexistent", updateDto))
                .ReturnsAsync(AdminService<AdminDto>.BadResult(
                    "Admin with ID nonexistent not found", 404));

            // Act
            var result = await _controller.UpdateAdmin("nonexistent", updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact]
        public async Task UpdateAdmin_Returns400_WhenValidationFails()
        {
            // Arrange
            var updateDto = new UpdateAdminDto
            {
                Email = "",  // brak emaila
                Name = "Admin",
                Role = "Admin",
                IsActive = true,
                TwoFactorEnabled = false
            };

            _mockService
                .Setup(s => s.UpdateAdminAsync("admin-1", updateDto))
                .ReturnsAsync(AdminService<AdminDto>.BadResult(
                    "Validation failed", 400,
                    new List<string> { "Email is required" }));

            // Act
            var result = await _controller.UpdateAdmin("admin-1", updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async Task UpdateAdmin_Returns400_WhenEmailTakenByAnotherAdmin()
        {
            // Arrange
            var updateDto = MakeUpdateDto();

            _mockService
                .Setup(s => s.UpdateAdminAsync("admin-1", updateDto))
                .ReturnsAsync(AdminService<AdminDto>.BadResult(
                    "Validation failed", 400,
                    new List<string> { "Another admin with this email already exists" }));

            // Act
            var result = await _controller.UpdateAdmin("admin-1", updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async Task UpdateAdmin_Returns500_WhenServiceFails()
        {
            // Arrange
            var updateDto = MakeUpdateDto();

            _mockService
                .Setup(s => s.UpdateAdminAsync("admin-1", updateDto))
                .ReturnsAsync(AdminService<AdminDto>.BadResult(
                    "Failed to update admin", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.UpdateAdmin("admin-1", updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task UpdateAdmin_CallsServiceWithCorrectIdAndDto()
        {
            // Arrange
            var updateDto = MakeUpdateDto();

            _mockService
                .Setup(s => s.UpdateAdminAsync("admin-5", updateDto))
                .ReturnsAsync(AdminService<AdminDto>.GoodResult("OK", 200, MakeAdminDto("admin-5")));

            // Act
            await _controller.UpdateAdmin("admin-5", updateDto);

            // Assert
            _mockService.Verify(s => s.UpdateAdminAsync("admin-5", updateDto), Times.Once);
        }

        // ───────────────────────────────────────────────
        // DELETE api/Admins/{id}
        // ───────────────────────────────────────────────

        [Fact]
        public async Task DeleteAdmin_Returns200_WhenDeletedSuccessfully()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAdminAsync("admin-1"))
                .ReturnsAsync(AdminService<bool>.GoodResult(
                    "Admin deleted successfully", 200, true));

            // Act
            var result = await _controller.DeleteAdmin("admin-1");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(true, objectResult.Value);
        }

        [Fact]
        public async Task DeleteAdmin_Returns404_WhenAdminNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAdminAsync("nonexistent"))
                .ReturnsAsync(AdminService<bool>.BadResult(
                    "Admin with ID nonexistent not found", 404, data: false));

            // Act
            var result = await _controller.DeleteAdmin("nonexistent");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteAdmin_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAdminAsync("admin-1"))
                .ReturnsAsync(AdminService<bool>.BadResult(
                    "Failed to delete admin", 500,
                    new List<string> { "Database error" }, false));

            // Act
            var result = await _controller.DeleteAdmin("admin-1");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteAdmin_CallsServiceWithCorrectId()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAdminAsync("admin-99"))
                .ReturnsAsync(AdminService<bool>.GoodResult("OK", 200, true));

            // Act
            await _controller.DeleteAdmin("admin-99");

            // Assert
            _mockService.Verify(s => s.DeleteAdminAsync("admin-99"), Times.Once);
        }

        [Fact]
        public async Task DeleteAdmin_DoesNotCallDelete_ForDifferentId()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteAdminAsync(It.IsAny<string>()))
                .ReturnsAsync(AdminService<bool>.GoodResult("OK", 200, true));

            // Act
            await _controller.DeleteAdmin("admin-1");

            // Assert
            _mockService.Verify(s => s.DeleteAdminAsync("admin-2"), Times.Never);
        }
    }
}