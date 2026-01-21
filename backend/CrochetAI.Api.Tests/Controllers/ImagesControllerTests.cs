using CrochetAI.Api.Controllers;
using CrochetAI.Api.DTOs;
using CrochetAI.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CrochetAI.Api.Tests.Controllers;

public class ImagesControllerTests
{
    private readonly Mock<IBlobStorageService> _mockBlobStorageService;
    private readonly Mock<ILogger<ImagesController>> _mockLogger;
    private readonly ImagesController _controller;

    public ImagesControllerTests()
    {
        _mockBlobStorageService = new Mock<IBlobStorageService>();
        _mockLogger = new Mock<ILogger<ImagesController>>();
        _controller = new ImagesController(_mockBlobStorageService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task UploadImage_ValidImage_ReturnsOk()
    {
        // Arrange
        var file = CreateMockFormFile("test.jpg", "image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 });
        _mockBlobStorageService.Setup(s => s.UploadImageAsync(It.IsAny<Stream>(), "test.jpg", "image/jpeg"))
            .ReturnsAsync("blob-name-123");
        _mockBlobStorageService.Setup(s => s.GenerateSasTokenAsync("blob-name-123", It.IsAny<TimeSpan>()))
            .ReturnsAsync("https://storage.azure.com/blob-name-123?sas-token");

        // Act
        var result = await _controller.UploadImage(file);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ImageUploadResponse>(okResult.Value);
        Assert.Equal("blob-name-123", response.BlobName);
        Assert.NotEmpty(response.TemporaryUrl);
    }

    [Fact]
    public async Task UploadImage_NoFile_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.UploadImage(null!);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UploadImage_FileTooLarge_ReturnsBadRequest()
    {
        // Arrange
        var largeFile = CreateMockFormFile("large.jpg", "image/jpeg", new byte[11 * 1024 * 1024]); // 11MB

        // Act
        var result = await _controller.UploadImage(largeFile);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("exceeds maximum", badRequest.Value?.ToString() ?? "");
    }

    [Fact]
    public async Task UploadImage_InvalidContentType_ReturnsBadRequest()
    {
        // Arrange
        var file = CreateMockFormFile("test.txt", "text/plain", new byte[] { 0xFF, 0xD8, 0xFF });

        // Act
        var result = await _controller.UploadImage(file);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("not allowed", badRequest.Value?.ToString() ?? "");
    }

    [Fact]
    public async Task DeleteImage_ValidBlobName_ReturnsNoContent()
    {
        // Arrange
        _mockBlobStorageService.Setup(s => s.DeleteImageAsync("blob-name-123"))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteImage("blob-name-123");

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteImage_NotFound_ReturnsNotFound()
    {
        // Arrange
        _mockBlobStorageService.Setup(s => s.DeleteImageAsync("nonexistent"))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteImage("nonexistent");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    private IFormFile CreateMockFormFile(string fileName, string contentType, byte[] content)
    {
        var stream = new MemoryStream(content);
        var formFile = new Mock<IFormFile>();
        formFile.Setup(f => f.FileName).Returns(fileName);
        formFile.Setup(f => f.ContentType).Returns(contentType);
        formFile.Setup(f => f.Length).Returns(content.Length);
        formFile.Setup(f => f.OpenReadStream()).Returns(stream);
        return formFile.Object;
    }
}
