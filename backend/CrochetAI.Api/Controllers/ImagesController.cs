using CrochetAI.Api.DTOs;
using CrochetAI.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrochetAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImagesController : ControllerBase
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly ILogger<ImagesController> _logger;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
    private static readonly byte[] JpegSignature = { 0xFF, 0xD8, 0xFF };
    private static readonly byte[] PngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
    private static readonly byte[] GifSignature = { 0x47, 0x49, 0x46, 0x38 };
    private static readonly byte[] WebpSignature = { 0x52, 0x49, 0x46, 0x46 };

    public ImagesController(
        IBlobStorageService blobStorageService,
        ILogger<ImagesController> logger)
    {
        _blobStorageService = blobStorageService;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        // Validate file size
        if (file.Length > MaxFileSize)
        {
            return BadRequest($"File size exceeds maximum allowed size of {MaxFileSize / (1024 * 1024)}MB");
        }

        // Validate content type
        if (!AllowedContentTypes.Contains(file.ContentType.ToLower()))
        {
            return BadRequest($"File type not allowed. Allowed types: {string.Join(", ", AllowedContentTypes)}");
        }

        // Validate magic bytes
        using (var stream = file.OpenReadStream())
        {
            var isValid = await ValidateMagicBytesAsync(stream, file.ContentType);
            if (!isValid)
            {
                return BadRequest("File content does not match declared file type");
            }

            // Reset stream position
            stream.Position = 0;

            try
            {
                var blobName = await _blobStorageService.UploadImageAsync(stream, file.FileName, file.ContentType);
                var temporaryUrl = await _blobStorageService.GenerateSasTokenAsync(blobName, TimeSpan.FromHours(1));

                return Ok(new ImageUploadResponse
                {
                    BlobName = blobName,
                    TemporaryUrl = temporaryUrl,
                    ContentType = file.ContentType,
                    SizeBytes = file.Length
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload image");
                return StatusCode(500, "Failed to upload image");
            }
        }
    }

    [HttpDelete("{blobName}")]
    public async Task<IActionResult> DeleteImage(string blobName)
    {
        try
        {
            var deleted = await _blobStorageService.DeleteImageAsync(blobName);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete image");
            return StatusCode(500, "Failed to delete image");
        }
    }

    private async Task<bool> ValidateMagicBytesAsync(Stream stream, string contentType)
    {
        var buffer = new byte[12];
        var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

        if (bytesRead < 4)
        {
            return false;
        }

        return contentType.ToLower() switch
        {
            "image/jpeg" => buffer[0] == JpegSignature[0] && buffer[1] == JpegSignature[1] && buffer[2] == JpegSignature[2],
            "image/png" => buffer.Take(8).SequenceEqual(PngSignature),
            "image/gif" => buffer.Take(4).SequenceEqual(GifSignature),
            "image/webp" => buffer.Take(4).SequenceEqual(WebpSignature) && buffer.Skip(8).Take(4).SequenceEqual(new byte[] { 0x57, 0x45, 0x42, 0x50 }),
            _ => false
        };
    }
}
