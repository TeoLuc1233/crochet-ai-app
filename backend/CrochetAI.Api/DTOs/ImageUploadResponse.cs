namespace CrochetAI.Api.DTOs;

public class ImageUploadResponse
{
    public string BlobName { get; set; } = string.Empty;
    public string TemporaryUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
}
