using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Backery.API.Controllers;
using WebApplication2.Backery.API.DTO;
using WebApplication2.Backery.Services.Services.Interfaces;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;

namespace WebApplication2.Tests.Controllers;

public class AccountControllerTests
{
    // ── helpers ──────────────────────────────────────────────────────────────

    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly Mock<IJwtTokenService> _tokenServiceMock;
    private readonly AccountController _sut;

    public AccountControllerTests()
    {
        // UserManager wymaga IUserStore – reszta parametrów może być null
        var store = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            store.Object, null, null, null, null, null, null, null, null);

        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
        _signInManagerMock = new Mock<SignInManager<User>>(
            _userManagerMock.Object, contextAccessor.Object,
            claimsFactory.Object, null, null, null, null);

        _tokenServiceMock = new Mock<IJwtTokenService>();

        _sut = new AccountController(
            _tokenServiceMock.Object,
            _userManagerMock.Object,
            _signInManagerMock.Object);
    }

    // ── LoginAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task LoginAsync_UserNotFound_ReturnsBadRequest()
    {
        _userManagerMock
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var result = await _sut.LoginAsync(new LoginDto { Email = "x@x.com", Password = "pass" });

        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsBadRequest()
    {
        var user = new User { Email = "x@x.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync("x@x.com")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, "wrong")).ReturnsAsync(false);

        var result = await _sut.LoginAsync(new LoginDto { Email = "x@x.com", Password = "wrong" });

        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task LoginAsync_2FAEnabled_ReturnsRequires2FA()
    {
        var user = new User { Email = "x@x.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync("x@x.com")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, "pass")).ReturnsAsync(true);
        _userManagerMock.Setup(m => m.GetTwoFactorEnabledAsync(user)).ReturnsAsync(true);
        _userManagerMock
            .Setup(m => m.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider))
            .ReturnsAsync("123456");

        var result = await _sut.LoginAsync(new LoginDto { Email = "x@x.com", Password = "pass" });

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = ok.Value!;
        value.GetType().GetProperty("requires2FA")!.GetValue(value).Should().Be(true);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsTokenAndUserId()
    {
        var user = new User { Id = "uid-1", Email = "x@x.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync("x@x.com")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, "pass")).ReturnsAsync(true);
        _userManagerMock.Setup(m => m.GetTwoFactorEnabledAsync(user)).ReturnsAsync(false);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
        _tokenServiceMock.Setup(t => t.CreateToken("uid-1", It.IsAny<IList<string>>())).Returns("jwt-token");

        var result = await _sut.LoginAsync(new LoginDto { Email = "x@x.com", Password = "pass" });

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var val = ok.Value!;
        val.GetType().GetProperty("access_token")!.GetValue(val).Should().Be("jwt-token");
        val.GetType().GetProperty("userId")!.GetValue(val).Should().Be("uid-1");
    }

    // ── Login2FAAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Login2FAAsync_UserNotFound_ReturnsBadRequest()
    {
        _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _sut.Login2FAAsync(new Login2FADto { Email = "x@x.com", Code2FA = "000" });

        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Login2FAAsync_NotAdmin_ReturnsForbid()
    {
        var user = new User { Email = "x@x.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync("x@x.com")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

        var result = await _sut.Login2FAAsync(new Login2FADto { Email = "x@x.com", Code2FA = "000" });

        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task Login2FAAsync_InvalidCode_ReturnsBadRequest()
    {
        var user = new User { Email = "x@x.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync("x@x.com")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });
        _userManagerMock
            .Setup(m => m.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, "bad"))
            .ReturnsAsync(false);

        var result = await _sut.Login2FAAsync(new Login2FADto { Email = "x@x.com", Code2FA = "bad" });

        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Login2FAAsync_ValidAdminCode_ReturnsToken()
    {
        var user = new User { Id = "admin-1", Email = "admin@x.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync("admin@x.com")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });
        _userManagerMock
            .Setup(m => m.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, "123456"))
            .ReturnsAsync(true);
        _tokenServiceMock.Setup(t => t.CreateToken("admin-1", It.IsAny<IList<string>>())).Returns("admin-jwt");

        var result = await _sut.Login2FAAsync(new Login2FADto { Email = "admin@x.com", Code2FA = "123456" });

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value!.GetType().GetProperty("access_token")!.GetValue(ok.Value).Should().Be("admin-jwt");
    }

    // ── RegisterAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task RegisterAsync_PasswordMismatch_ReturnsBadRequest()
    {
        var dto = new RegisterDto
        {
            Email = "u@x.com",
            Username = "user1",
            Password = "Abc123!",
            ConfirmPassword = "Different!"
        };

        var result = await _sut.RegisterAsync(dto);

        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task RegisterAsync_CreateFails_ReturnsBadRequestWithErrors()
    {
        var dto = new RegisterDto
        {
            Email = "u@x.com",
            Username = "user1",
            Password = "Abc123!",
            ConfirmPassword = "Abc123!"
        };
        var errors = new[] { new IdentityError { Code = "DuplicateEmail", Description = "Email taken" } };
        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<User>(), "Abc123!"))
            .ReturnsAsync(IdentityResult.Failed(errors));

        var result = await _sut.RegisterAsync(dto);

        var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        bad.Value.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public async Task RegisterAsync_Success_ReturnsOk()
    {
        var dto = new RegisterDto
        {
            Email = "u@x.com",
            Username = "user1",
            Password = "Abc123!",
            ConfirmPassword = "Abc123!"
        };
        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<User>(), "Abc123!"))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock
            .Setup(m => m.AddToRoleAsync(It.IsAny<User>(), "User"))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock
            .Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()))
            .ReturnsAsync("email-confirm-token");

        var result = await _sut.RegisterAsync(dto);

        result.Should().BeOfType<OkResult>();
    }

    // ── ConfirmEmail ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ConfirmEmail_UserNotFound_ReturnsBadRequest()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync("uid")).ReturnsAsync((User?)null);

        var result = await _sut.CofirmEmail("uid", "token");

        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task ConfirmEmail_InvalidToken_ReturnsBadRequest()
    {
        var user = new User { Id = "uid" };
        _userManagerMock.Setup(m => m.FindByIdAsync("uid")).ReturnsAsync(user);
        _userManagerMock
            .Setup(m => m.ConfirmEmailAsync(user, "bad-token"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid token" }));

        var result = await _sut.CofirmEmail("uid", "bad-token");

        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task ConfirmEmail_Valid_ReturnsOk()
    {
        var user = new User { Id = "uid" };
        _userManagerMock.Setup(m => m.FindByIdAsync("uid")).ReturnsAsync(user);
        _userManagerMock
            .Setup(m => m.ConfirmEmailAsync(user, "good-token"))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _sut.CofirmEmail("uid", "good-token");

        result.Should().BeOfType<OkResult>();
    }

    // ── ForgotPassword ────────────────────────────────────────────────────────

    [Fact]
    public async Task ForgotPassword_EmptyEmail_ReturnsBadRequest()
    {
        var result = await _sut.ForgotPassword(new ForgotPasswordDto { Email = "" });

        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task ForgotPassword_UserNotFound_ReturnsOkSilently()
    {
        // Celowo zwracamy Ok (nie ujawniamy istnienia konta)
        _userManagerMock.Setup(m => m.FindByEmailAsync("ghost@x.com")).ReturnsAsync((User?)null);

        var result = await _sut.ForgotPassword(new ForgotPasswordDto { Email = "ghost@x.com" });

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task ForgotPassword_UserExists_ReturnsTokenAndUserId()
    {
        var user = new User { Id = "uid-2", Email = "real@x.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync("real@x.com")).ReturnsAsync(user);
        _userManagerMock
            .Setup(m => m.GeneratePasswordResetTokenAsync(user))
            .ReturnsAsync("reset-token");

        var result = await _sut.ForgotPassword(new ForgotPasswordDto { Email = "real@x.com" });

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var val = ok.Value!;
        val.GetType().GetProperty("token")!.GetValue(val).Should().Be("reset-token");
        val.GetType().GetProperty("userId")!.GetValue(val).Should().Be("uid-2");
    }

    // ── ResetPassword ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ResetPassword_MissingFields_ReturnsBadRequest()
    {
        var result = await _sut.ResetPassword(
            new ResetPasswordDto { Email = "", Token = "t", NewPassword = "p" });

        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task ResetPassword_UserNotFound_ReturnsBadRequest()
    {
        _userManagerMock.Setup(m => m.FindByEmailAsync("x@x.com")).ReturnsAsync((User?)null);

        var result = await _sut.ResetPassword(
            new ResetPasswordDto { Email = "x@x.com", Token = "t", NewPassword = "NewPass1!" });

        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task ResetPassword_InvalidToken_ReturnsBadRequestWithErrors()
    {
        var user = new User { Email = "x@x.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync("x@x.com")).ReturnsAsync(user);
        var errors = new[] { new IdentityError { Description = "Invalid token" } };
        _userManagerMock
            .Setup(m => m.ResetPasswordAsync(user, "bad", "NewPass1!"))
            .ReturnsAsync(IdentityResult.Failed(errors));

        var result = await _sut.ResetPassword(
            new ResetPasswordDto { Email = "x@x.com", Token = "bad", NewPassword = "NewPass1!" });

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ResetPassword_Valid_ReturnsOk()
    {
        var user = new User { Email = "x@x.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync("x@x.com")).ReturnsAsync(user);
        _userManagerMock
            .Setup(m => m.ResetPasswordAsync(user, "good-token", "NewPass1!"))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _sut.ResetPassword(
            new ResetPasswordDto { Email = "x@x.com", Token = "good-token", NewPassword = "NewPass1!" });

        result.Should().BeOfType<OkResult>();
    }
}