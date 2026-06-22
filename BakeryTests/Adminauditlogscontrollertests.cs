using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Controllers;
using WebApplication2.DTO;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Tests.Controllers;

public class AdminAuditLogsControllerTests
{
    // ── setup ─────────────────────────────────────────────────────────────────

    private readonly Mock<IAdminAuditLogsService> _serviceMock = new();
    private readonly AdminAuditLogsController _sut;

    public AdminAuditLogsControllerTests()
    {
        _sut = new AdminAuditLogsController(_serviceMock.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ── fabryki ResultsService ────────────────────────────────────────────────

    private static ResultsService<T> Good<T>(T data, int code = 200) =>
        ResultsService<T>.GoodResult("ok", code, data);

    private static ResultsService<T> Bad<T>(int code, string msg) =>
        ResultsService<T>.BadResult(msg, code, new List<string> { msg });

    // ── GetAdminAuditLogs ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetAdminAuditLogs_ServiceSucceeds_Returns200WithData()
    {
        var list = new List<AdminAuditLogListItemDto>
        {
            new() { LogId = 1, Action = "LOGIN", AdminEmail = "admin@x.com" }
        };
        _serviceMock
            .Setup(s => s.GetAdminAuditLogsDtoAsync())
            .ReturnsAsync(Good(list));

        var result = await _sut.GetAdminAuditLogs();

        var obj = result.Result.Should().BeOfType<ObjectResult>().Subject;
        obj.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(list);
    }

    [Fact]
    public async Task GetAdminAuditLogs_ServiceFails_Returns500WithErrorBody()
    {
        _serviceMock
            .Setup(s => s.GetAdminAuditLogsDtoAsync())
            .ReturnsAsync(Bad<List<AdminAuditLogListItemDto>>(500, "DB error"));

        var result = await _sut.GetAdminAuditLogs();

        var obj = result.Result.Should().BeOfType<ObjectResult>().Subject;
        obj.StatusCode.Should().Be(500);
        obj.Value.Should().BeEquivalentTo(new
        {
            success = false,
            message = "DB error",
            errors = new[] { "DB error" }
        });
    }

    // ── GetAdminAuditLog (po id) ──────────────────────────────────────────────

    [Fact]
    public async Task GetAdminAuditLog_Found_Returns200WithData()
    {
        var dto = new AdminAuditLogResponseDto { LogId = 5, Action = "DELETE" };
        _serviceMock
            .Setup(s => s.GetAdminAuditLogByIdDtoAsync(5))
            .ReturnsAsync(Good(dto));

        var result = await _sut.GetAdminAuditLog(5);

        var obj = result.Result.Should().BeOfType<ObjectResult>().Subject;
        obj.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetAdminAuditLog_NotFound_Returns404()
    {
        _serviceMock
            .Setup(s => s.GetAdminAuditLogByIdDtoAsync(99))
            .ReturnsAsync(Bad<AdminAuditLogResponseDto>(404, "Nie znaleziono"));

        var result = await _sut.GetAdminAuditLog(99);

        var obj = result.Result.Should().BeOfType<ObjectResult>().Subject;
        obj.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(new
        {
            success = false,
            message = "Nie znaleziono",
            errors = new[] { "Nie znaleziono" }
        });
    }

    [Fact]
    public async Task GetAdminAuditLog_ServiceFails_Returns500()
    {
        _serviceMock
            .Setup(s => s.GetAdminAuditLogByIdDtoAsync(1))
            .ReturnsAsync(Bad<AdminAuditLogResponseDto>(500, "DB error"));

        var result = await _sut.GetAdminAuditLog(1);

        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(500);
    }

    // ── CreateAdminAuditLog ───────────────────────────────────────────────────

    [Fact]
    public async Task CreateAdminAuditLog_ServiceSucceeds_Returns201WithData()
    {
        var createDto = new CreateAdminAuditLogDto
        {
            AdminId = "1",
            Action = "DELETE",
            TargetType = "User",
            TargetId = 42
        };
        var resp = new AdminAuditLogResponseDto { LogId = 10, Action = "DELETE" };
        _serviceMock
            .Setup(s => s.CreateAdminAuditLogAsync(createDto))
            .ReturnsAsync(Good(resp, 201));

        var result = await _sut.CreateAdminAuditLog(createDto);

        var obj = result.Result.Should().BeOfType<ObjectResult>().Subject;
        obj.StatusCode.Should().Be(201);
        obj.Value.Should().BeEquivalentTo(resp);
    }

    [Fact]
    public async Task CreateAdminAuditLog_ServiceFails_Returns500()
    {
        var createDto = new CreateAdminAuditLogDto { AdminId = "1", Action = "X" };
        _serviceMock
            .Setup(s => s.CreateAdminAuditLogAsync(createDto))
            .ReturnsAsync(Bad<AdminAuditLogResponseDto>(500, "DB error"));

        var result = await _sut.CreateAdminAuditLog(createDto);

        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(500);
    }

    // ── UpdateAdminAuditLog ───────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAdminAuditLog_ServiceSucceeds_Returns204()
    {
        var updateDto = new UpdateAdminAuditLogDto { Action = "UPDATE" };
        _serviceMock
            .Setup(s => s.UpdateAdminAuditLogAsync(3, updateDto))
            .ReturnsAsync(Good<object>(null!, 204));

        var result = await _sut.UpdateAdminAuditLog(3, updateDto);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task UpdateAdminAuditLog_NotFound_Returns404()
    {
        var updateDto = new UpdateAdminAuditLogDto { Action = "UPDATE" };
        _serviceMock
            .Setup(s => s.UpdateAdminAuditLogAsync(99, updateDto))
            .ReturnsAsync(Bad<object>(404, "Nie znaleziono"));

        var result = await _sut.UpdateAdminAuditLog(99, updateDto);

        var obj = result.Should().BeOfType<ObjectResult>().Subject;
        obj.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(new
        {
            success = false,
            message = "Nie znaleziono",
            errors = new[] { "Nie znaleziono" }
        });
    }

    [Fact]
    public async Task UpdateAdminAuditLog_Conflict_Returns409()
    {
        var updateDto = new UpdateAdminAuditLogDto { Action = "UPDATE" };
        _serviceMock
            .Setup(s => s.UpdateAdminAuditLogAsync(3, updateDto))
            .ReturnsAsync(Bad<object>(409, "Konflikt współbieżności"));

        var result = await _sut.UpdateAdminAuditLog(3, updateDto);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task UpdateAdminAuditLog_ServiceFails_Returns500()
    {
        var updateDto = new UpdateAdminAuditLogDto { Action = "UPDATE" };
        _serviceMock
            .Setup(s => s.UpdateAdminAuditLogAsync(3, updateDto))
            .ReturnsAsync(Bad<object>(500, "DB error"));

        var result = await _sut.UpdateAdminAuditLog(3, updateDto);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(500);
    }

    // ── DeleteAdminAuditLog ───────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAdminAuditLog_ServiceSucceeds_Returns204()
    {
        _serviceMock
            .Setup(s => s.DeleteAdminAuditLogAsync(5))
            .ReturnsAsync(Good<object>(null!, 204));

        var result = await _sut.DeleteAdminAuditLog(5);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task DeleteAdminAuditLog_NotFound_Returns404()
    {
        _serviceMock
            .Setup(s => s.DeleteAdminAuditLogAsync(99))
            .ReturnsAsync(Bad<object>(404, "Nie znaleziono"));

        var result = await _sut.DeleteAdminAuditLog(99);

        var obj = result.Should().BeOfType<ObjectResult>().Subject;
        obj.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(new
        {
            success = false,
            message = "Nie znaleziono",
            errors = new[] { "Nie znaleziono" }
        });
    }

    [Fact]
    public async Task DeleteAdminAuditLog_ServiceFails_Returns500()
    {
        _serviceMock
            .Setup(s => s.DeleteAdminAuditLogAsync(1))
            .ReturnsAsync(Bad<object>(500, "DB error"));

        var result = await _sut.DeleteAdminAuditLog(1);

        var obj = result.Should().BeOfType<ObjectResult>().Subject;
        obj.StatusCode.Should().Be(500);
        obj.Value.Should().BeEquivalentTo(new
        {
            success = false,
            message = "DB error",
            errors = new[] { "DB error" }
        });
    }
}