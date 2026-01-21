using System.Security.Claims;
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
        [FromQuery] bool? isPremium = null)
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
        if (isPremium.HasValue)
        {
            patterns = patterns.Where(p => p.IsPremium == isPremium.Value);
        }

        // Filter out premium patterns for free users
        if (!canAccessPremium)
        {
            patterns = patterns.Where(p => !p.IsPremium);
        }

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

        var dto = new PatternDto
        {
            Id = pattern.Id,
            Title = pattern.Title,
            Description = pattern.Description,
            Difficulty = pattern.Difficulty,
            Category = pattern.Category,
            Materials = pattern.Materials ?? new List<string>(),
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
        [FromQuery] string? category = null)
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

        // Filter out premium patterns for free users
        if (!canAccessPremium)
        {
            patterns = patterns.Where(p => !p.IsPremium);
        }

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
        var tier = User.FindFirstValue("SubscriptionTier");
        return tier ?? "Free";
    }
}
