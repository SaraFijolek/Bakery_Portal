using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text;
using WebApplication2.Backery.API.DTO;
using WebApplication2.Controllers;
using WebApplication2.DTO;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Tests.Controllers;

public class AdMediaControllerTests
{
    // ── setup ─────────────────────────────────────────────────────────────────

    private readonly Mock<IAdMadiaService> _serviceMock = new();
    private readonly AdMediaController _sut;

    public AdMediaControllerTests()
    {
        _sut = new AdMediaController(_serviceMock.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ── fabryki ResultService ─────────────────────────────────────────────────

    private static ResultService<T> Good<T>(T data, int code = 200) =>
        ResultService<T>.GoodResult("ok", code, data);

    private static ResultService<T> Bad<T>(int code, string msg) =>
        ResultService<T>.BadResult(msg, code, new List<string> { msg });

    // ── GetAdMedia (lista) ────────────────────────────────────────────────────

    [Fact]
    public async Task GetAdMedia_ServiceSucceeds_ReturnsData()
    {
        IEnumerable<AdMediaResponseDto> list = new List<AdMediaResponseDto>
        {
            new() { MediaId = 1, Url = "http://x.com/a.jpg" }
        };
        _serviceMock
            .Setup(s => s.GetAllAdMediaAsync())
            .ReturnsAsync(Good(list));

        var result = await _sut.GetAdMedia();

        var obj = result.Result.Should().BeOfType<ObjectResult>().Subject;
        obj.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(list);
    }

    [Fact]
    public async Task GetAdMedia_ServiceFails_ReturnsErrorBody()
    {
        _serviceMock
            .Setup(s => s.GetAllAdMediaAsync())
            .ReturnsAsync(Bad<IEnumerable<AdMediaResponseDto>>(500, "DB error"));

        var result = await _sut.GetAdMedia();

        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(500);
    }

    // ── GetAdMedia (po id) ────────────────────────────────────────────────────

    [Fact]
    public async Task GetAdMediaById_Found_Returns200()
    {
        var dto = new AdMediaResponseDto { MediaId = 5, Url = "http://x.com/b.mp4" };
        _serviceMock
            .Setup(s => s.GetAdMediaByIdAsync(5))
            .ReturnsAsync(Good(dto));

        var result = await _sut.GetAdMedia(5);

        var obj = result.Result.Should().BeOfType<ObjectResult>().Subject;
        obj.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetAdMediaById_NotFound_Returns404()
    {
        _serviceMock
            .Setup(s => s.GetAdMediaByIdAsync(99))
            .ReturnsAsync(Bad<AdMediaResponseDto>(404, "Not found"));

        var result = await _sut.GetAdMedia(99);

        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(404);
    }

    // ── CreateAdMedia ─────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAdMedia_InvalidModelState_ReturnsBadRequest()
    {
        _sut.ModelState.AddModelError("Url", "Required");

        var result = await _sut.CreateAdMedia(new CreateAdMediaDto());

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateAdMedia_ServiceSucceeds_Returns201()
    {
        var dto = new CreateAdMediaDto { AdId = 1, Url = "http://x.com/a.jpg", MediaType = "image" };
        var resp = new AdMediaResponseDto { MediaId = 10, Url = dto.Url };
        _serviceMock
            .Setup(s => s.CreateAdMediaAsync(dto))
            .ReturnsAsync(Good(resp, 201));

        var result = await _sut.CreateAdMedia(dto);

        var obj = result.Result.Should().BeOfType<ObjectResult>().Subject;
        obj.StatusCode.Should().Be(201);
        obj.Value.Should().BeEquivalentTo(resp);
    }

    [Fact]
    public async Task CreateAdMedia_ServiceFails_Returns500()
    {
        var dto = new CreateAdMediaDto { AdId = 1, Url = "bad", MediaType = "image" };
        _serviceMock
            .Setup(s => s.CreateAdMediaAsync(dto))
            .ReturnsAsync(Bad<AdMediaResponseDto>(500, "DB error"));

        var result = await _sut.CreateAdMedia(dto);

        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(500);
    }

    // ── UploadAdMedia ─────────────────────────────────────────────────────────

    private static IFormFile MakeFile(string name, string contentType, string content = "fake-data")
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "File", name)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    [Fact]
    public async Task UploadAdMedia_NullFile_ReturnsBadRequest()
    {
        var result = await _sut.UploadAdMedia(new UploadAdMediaDto { File = null!, AdId = 1 });

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("Brak pliku");
    }

    [Fact]
    public async Task UploadAdMedia_EmptyFile_ReturnsBadRequest()
    {
        var empty = new FormFile(Stream.Null, 0, 0, "File", "empty.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };

        var result = await _sut.UploadAdMedia(new UploadAdMediaDto { File = empty, AdId = 1 });

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("Brak pliku");
    }

    [Fact]
    public async Task UploadAdMedia_UnsupportedContentType_ReturnsBadRequest()
    {
        var file = MakeFile("test.gif", "image/gif");
        var result = await _sut.UploadAdMedia(new UploadAdMediaDto { File = file, AdId = 1 });

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("Nieobsługiwany typ pliku");
    }

    [Theory]
    [InlineData("photo.jpg", "image/jpeg", "image")]
    [InlineData("photo.png", "image/png", "image")]
    [InlineData("clip.mp4", "video/mp4", "video")]
    [InlineData("clip.mov", "video/quicktime", "video")]
    public async Task UploadAdMedia_ValidFile_CallsServiceAndReturnsOk(
        string fileName, string contentType, string expectedMediaType)
    {
        var file = MakeFile(fileName, contentType);
        var resp = new AdMediaResponseDto { MediaId = 1, Url = "http://localhost/uploads/x" };

        _serviceMock
            .Setup(s => s.CreateAdMediaAsync(It.IsAny<CreateAdMediaDto>()))
            .ReturnsAsync(Good(resp, 201));

        var tmpDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var origDir = Directory.GetCurrentDirectory();
        Directory.CreateDirectory(Path.Combine(tmpDir, "wwwroot", "uploads"));

        try
        {
            Directory.SetCurrentDirectory(tmpDir);

            var result = await _sut.UploadAdMedia(new UploadAdMediaDto { File = file, AdId = 7 });

            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(resp);

            _serviceMock.Verify(s => s.CreateAdMediaAsync(
                It.Is<CreateAdMediaDto>(d =>
                    d.AdId == 7 &&
                    d.MediaType == expectedMediaType)),
                Times.Once);
        }
        finally
        {
            Directory.SetCurrentDirectory(origDir);
            Directory.Delete(tmpDir, recursive: true);
        }
    }

    [Fact]
    public async Task UploadAdMedia_ServiceFails_ReturnsServiceStatusCode()
    {
        var file = MakeFile("photo.jpg", "image/jpeg");
        _serviceMock
            .Setup(s => s.CreateAdMediaAsync(It.IsAny<CreateAdMediaDto>()))
            .ReturnsAsync(Bad<AdMediaResponseDto>(422, "Unprocessable"));

        var tmpDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var origDir = Directory.GetCurrentDirectory();
        Directory.CreateDirectory(Path.Combine(tmpDir, "wwwroot", "uploads"));

        try
        {
            Directory.SetCurrentDirectory(tmpDir);

            var result = await _sut.UploadAdMedia(new UploadAdMediaDto { File = file, AdId = 1 });

            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(422);
        }
        finally
        {
            Directory.SetCurrentDirectory(origDir);
            Directory.Delete(tmpDir, recursive: true);
        }
    }

    // ── UpdateAdMedia ─────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAdMedia_InvalidModelState_ReturnsBadRequest()
    {
        _sut.ModelState.AddModelError("Url", "Required");

        var result = await _sut.UpdateAdMedia(1, new UpdateAdMediaDto { MediaId = 1 });

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateAdMedia_IdMismatch_ReturnsBadRequest()
    {
        var result = await _sut.UpdateAdMedia(1, new UpdateAdMediaDto { MediaId = 99 });

        var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        bad.Value.Should().BeEquivalentTo(new
        {
            success = false,
            message = "ID w URL nie zgadza się z ID w obiekcie",
            errors = new[] { "ID mismatch between URL and request body" }
        });
    }

    [Fact]
    public async Task UpdateAdMedia_ServiceSucceeds_Returns200()
    {
        var dto = new UpdateAdMediaDto { MediaId = 3, Url = "http://x.com/new.jpg" };
        _serviceMock
            .Setup(s => s.UpdateAdMediaAsync(3, dto))
            .ReturnsAsync(Good(true));

        var result = await _sut.UpdateAdMedia(3, dto);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task UpdateAdMedia_NotFound_Returns404()
    {
        var dto = new UpdateAdMediaDto { MediaId = 3 };
        _serviceMock
            .Setup(s => s.UpdateAdMediaAsync(3, dto))
            .ReturnsAsync(Bad<bool>(404, "Not found"));

        var result = await _sut.UpdateAdMedia(3, dto);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(404);
    }

    // ── DeleteAdMedia ─────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAdMedia_ServiceSucceeds_Returns200()
    {
        _serviceMock
            .Setup(s => s.DeleteAdMediaAsync(5))
            .ReturnsAsync(Good(true, 200));

        var result = await _sut.DeleteAdMedia(5);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DeleteAdMedia_NotFound_Returns404()
    {
        _serviceMock
            .Setup(s => s.DeleteAdMediaAsync(99))
            .ReturnsAsync(Bad<bool>(404, "Not found"));

        var result = await _sut.DeleteAdMedia(99);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task DeleteAdMedia_ServiceFails_Returns500()
    {
        _serviceMock
            .Setup(s => s.DeleteAdMediaAsync(1))
            .ReturnsAsync(Bad<bool>(500, "DB error"));

        var result = await _sut.DeleteAdMedia(1);

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