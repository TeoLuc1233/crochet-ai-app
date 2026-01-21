using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace CrochetAI.Api.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(
        IConfiguration configuration,
        ILogger<BlobStorageService> logger)
    {
        var connectionString = configuration["Azure:Storage:ConnectionString"];
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Azure Storage connection string is not configured");
        }

        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerName = configuration["Azure:Storage:ContainerName"] ?? "crochet-images";
        _logger = logger;

        // Ensure container exists
        EnsureContainerExistsAsync().Wait();
    }

    private async Task EnsureContainerExistsAsync()
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create blob container");
            throw;
        }
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobName = $"{Guid.NewGuid()}_{fileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(imageStream, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            });

            return blobName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload image");
            throw;
        }
    }

    public async Task<bool> DeleteImageAsync(string blobName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            return await blobClient.DeleteIfExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete image");
            return false;
        }
    }

    public async Task<string> GenerateSasTokenAsync(string blobName, TimeSpan expiry)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = _containerName,
                    BlobName = blobName,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.Add(expiry)
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                var sasToken = blobClient.GenerateSasUri(sasBuilder);
                return sasToken.ToString();
            }

            throw new FileNotFoundException($"Blob {blobName} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate SAS token");
            throw;
        }
    }

    public async Task<bool> ImageExistsAsync(string blobName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            return await blobClient.ExistsAsync();
        }
        catch
        {
            return false;
        }
    }
}
