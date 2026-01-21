namespace CrochetAI.Api.DTOs;

public class PatternDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Materials { get; set; } = new();
    public string Instructions { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsPremium { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PatternListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsPremium { get; set; }
}

public class PatternListResponse
{
    public List<PatternListDto> Patterns { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
