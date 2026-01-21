namespace CrochetAI.Api.Services;

public interface IClaudeVisionService
{
    Task<string> AnalyzeImageAsync(string imageUrl, CancellationToken cancellationToken = default);
}
