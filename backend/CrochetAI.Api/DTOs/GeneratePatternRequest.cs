namespace CrochetAI.Api.DTOs;

public class GeneratePatternRequest
{
    public string ImageUrl { get; set; } = string.Empty;
    public string? UserPreferences { get; set; }
}

public class GeneratePatternResponse
{
    public int PatternId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Materials { get; set; } = new();
    public string Instructions { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
