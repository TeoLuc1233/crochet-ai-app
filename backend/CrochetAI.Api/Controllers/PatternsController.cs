using System.Security.Claims;
using System.Text.Json;
using CrochetAI.Api.DTOs;
using CrochetAI.Api.Models;
using CrochetAI.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrochetAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatternsController : ControllerBase
{
    private readonly IPatternRepository _patternRepository;
    private readonly ILogger<PatternsController> _logger;

    public PatternsController(
        IPatternRepository patternRepository,
        ILogger<PatternsController> logger)
    {
        _patternRepository = patternRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetPatterns(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? difficulty = null,
        [FromQuery] string? category = null,
        [FromQuery] string? material = null,
        [FromQuery] bool? isPremium = null,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "desc")
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var userTier = GetUserSubscriptionTier();
        var canAccessPremium = userTier != "Free";

        var patterns = await _patternRepository.GetAllAsync();
        
        // Apply filters
        if (difficulty != null)
        {
            patterns = patterns.Where(p => p.Difficulty == difficulty);
        }
        if (category != null)
        {
            patterns = patterns.Where(p => p.Category == category);
        }
        if (material != null)
        {
            patterns = patterns.Where(p => p.Materials.Contains(material, StringComparison.OrdinalIgnoreCase));
        }
        if (isPremium.HasValue)
        {
            patterns = patterns.Where(p => p.IsPremium == isPremium.Value);
        }

        // Filter out premium patterns for free users
        if (!canAccessPremium)
        {
            patterns = patterns.Where(p => !p.IsPremium);
        }

        // Apply sorting
        patterns = sortBy.ToLower() switch
        {
            "popularity" => sortOrder.ToLower() == "asc" 
                ? patterns.OrderBy(p => p.ViewCount)
                : patterns.OrderByDescending(p => p.ViewCount),
            "difficulty" => sortOrder.ToLower() == "asc"
                ? patterns.OrderBy(p => p.Difficulty)
                : patterns.OrderByDescending(p => p.Difficulty),
            "title" => sortOrder.ToLower() == "asc"
                ? patterns.OrderBy(p => p.Title)
                : patterns.OrderByDescending(p => p.Title),
            _ => sortOrder.ToLower() == "asc"
                ? patterns.OrderBy(p => p.CreatedAt)
                : patterns.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = patterns.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var pagedPatterns = patterns
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PatternListDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Difficulty = p.Difficulty,
                Category = p.Category,
                ImageUrl = p.ImageUrl,
                IsPremium = p.IsPremium
            })
            .ToList();

        return Ok(new PatternListResponse
        {
            Patterns = pagedPatterns,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPattern(int id)
    {
        var pattern = await _patternRepository.GetByIdAsync(id);
        if (pattern == null)
        {
            return NotFound();
        }

        var userTier = GetUserSubscriptionTier();
        var canAccessPremium = userTier != "Free";

        // Check premium access
        if (pattern.IsPremium && !canAccessPremium)
        {
            return Forbid("Premium subscription required to access this pattern");
        }

        // Parse Materials JSON string to List<string>
        var materials = new List<string>();
        if (!string.IsNullOrEmpty(pattern.Materials))
        {
            try
            {
                var materialsJson = JsonSerializer.Deserialize<Dictionary<string, object>>(pattern.Materials);
                if (materialsJson != null)
                {
                    // Extract materials from JSON structure
                    foreach (var item in materialsJson)
                    {
                        if (item.Value is JsonElement element)
                        {
                            if (element.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var arrayItem in element.EnumerateArray())
                                {
                                    materials.Add(arrayItem.GetString() ?? string.Empty);
                                }
                            }
                            else
                            {
                                materials.Add(element.GetString() ?? string.Empty);
                            }
                        }
                        else
                        {
                            materials.Add(item.Value?.ToString() ?? string.Empty);
                        }
                    }
                }
            }
            catch
            {
                // If JSON parsing fails, treat as single string
                materials.Add(pattern.Materials);
            }
        }

        var dto = new PatternDto
        {
            Id = pattern.Id,
            Title = pattern.Title,
            Description = pattern.Description ?? string.Empty,
            Difficulty = pattern.Difficulty,
            Category = pattern.Category,
            Materials = materials,
            Instructions = pattern.Instructions,
            ImageUrl = pattern.ImageUrl,
            IsPremium = pattern.IsPremium,
            CreatedAt = pattern.CreatedAt,
            UpdatedAt = pattern.UpdatedAt
        };

        return Ok(dto);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchPatterns(
        [FromQuery] string? query = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? difficulty = null,
        [FromQuery] string? category = null,
        [FromQuery] string? material = null,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "desc")
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var userTier = GetUserSubscriptionTier();
        var canAccessPremium = userTier != "Free";

        var patterns = await _patternRepository.GetAllAsync();

        // Apply search query
        if (!string.IsNullOrWhiteSpace(query))
        {
            patterns = patterns.Where(p =>
                p.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        // Apply filters
        if (difficulty != null)
        {
            patterns = patterns.Where(p => p.Difficulty == difficulty);
        }
        if (category != null)
        {
            patterns = patterns.Where(p => p.Category == category);
        }
        if (material != null)
        {
            patterns = patterns.Where(p => p.Materials.Contains(material, StringComparison.OrdinalIgnoreCase));
        }

        // Filter out premium patterns for free users
        if (!canAccessPremium)
        {
            patterns = patterns.Where(p => !p.IsPremium);
        }

        // Apply sorting
        patterns = sortBy.ToLower() switch
        {
            "popularity" => sortOrder.ToLower() == "asc" 
                ? patterns.OrderBy(p => p.ViewCount)
                : patterns.OrderByDescending(p => p.ViewCount),
            "difficulty" => sortOrder.ToLower() == "asc"
                ? patterns.OrderBy(p => p.Difficulty)
                : patterns.OrderByDescending(p => p.Difficulty),
            "title" => sortOrder.ToLower() == "asc"
                ? patterns.OrderBy(p => p.Title)
                : patterns.OrderByDescending(p => p.Title),
            _ => sortOrder.ToLower() == "asc"
                ? patterns.OrderBy(p => p.CreatedAt)
                : patterns.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = patterns.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var pagedPatterns = patterns
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PatternListDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Difficulty = p.Difficulty,
                Category = p.Category,
                ImageUrl = p.ImageUrl,
                IsPremium = p.IsPremium
            })
            .ToList();

        return Ok(new PatternListResponse
        {
            Patterns = pagedPatterns,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        });
    }

    private string GetUserSubscriptionTier()
    {
        if (User?.Identity?.IsAuthenticated != true)
        {
            return "Free";
        }
        var tier = User.FindFirstValue("SubscriptionTier");
        return tier ?? "Free";
    }
}
