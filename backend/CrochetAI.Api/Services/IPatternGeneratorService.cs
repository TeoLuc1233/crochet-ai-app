namespace CrochetAI.Api.Services;

public class PatternGenerationRequest
{
    public string ImageAnalysis { get; set; } = string.Empty;
    public string? UserPreferences { get; set; }
}

public class PatternGenerationResult
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Materials { get; set; } = new();
    public string Instructions { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public interface IPatternGeneratorService
{
    Task<PatternGenerationResult> GeneratePatternAsync(PatternGenerationRequest request, CancellationToken cancellationToken = default);
}
