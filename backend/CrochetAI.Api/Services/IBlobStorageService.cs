namespace CrochetAI.Api.Services;

public interface IBlobStorageService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType);
    Task<bool> DeleteImageAsync(string blobName);
    Task<string> GenerateSasTokenAsync(string blobName, TimeSpan expiry);
    Task<bool> ImageExistsAsync(string blobName);
}
