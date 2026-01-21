using System.Security.Claims;
using CrochetAI.Api.Data;
using CrochetAI.Api.DTOs;
using CrochetAI.Api.Models;
using CrochetAI.Api.Repositories;
using CrochetAI.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrochetAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIController : ControllerBase
{
    private readonly IClaudeVisionService _claudeVisionService;
    private readonly IPatternGeneratorService _patternGeneratorService;
    private readonly IPatternRepository _patternRepository;
    private readonly IAIGenerationRepository _aiGenerationRepository;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AIController> _logger;

    public AIController(
        IClaudeVisionService claudeVisionService,
        IPatternGeneratorService patternGeneratorService,
        IPatternRepository patternRepository,
        IAIGenerationRepository aiGenerationRepository,
        ApplicationDbContext context,
        ILogger<AIController> logger)
    {
        _claudeVisionService = claudeVisionService;
        _patternGeneratorService = patternGeneratorService;
        _patternRepository = patternRepository;
        _aiGenerationRepository = aiGenerationRepository;
        _context = context;
        _logger = logger;
    }

    [HttpPost("generate-pattern")]
    public async Task<IActionResult> GeneratePattern([FromBody] GeneratePatternRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            // Analyze image
            var analysis = await _claudeVisionService.AnalyzeImageAsync(request.ImageUrl);

            // Generate pattern
            var patternResult = await _patternGeneratorService.GeneratePatternAsync(
                new PatternGenerationRequest
                {
                    ImageAnalysis = analysis,
                    UserPreferences = request.UserPreferences
                });

            // Save pattern to database
            var pattern = new Pattern
            {
                Title = patternResult.Title,
                Description = patternResult.Description,
                Difficulty = patternResult.Difficulty,
                Category = patternResult.Category,
                Materials = System.Text.Json.JsonSerializer.Serialize(patternResult.Materials),
                Instructions = patternResult.Instructions,
                ImageUrl = request.ImageUrl,
                IsPremium = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _patternRepository.AddAsync(pattern);
            await _context.SaveChangesAsync();

            // Track AI generation
            var aiGeneration = new AIGeneration
            {
                UserId = userId,
                ImageUrl = request.ImageUrl,
                ImageHash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(request.ImageUrl)).ToString() ?? "",
                AnalysisResult = System.Text.Json.JsonSerializer.Serialize(analysis),
                GeneratedPattern = System.Text.Json.JsonSerializer.Serialize(patternResult),
                CreatedAt = DateTime.UtcNow
            };
            await _aiGenerationRepository.AddAsync(aiGeneration);
            await _context.SaveChangesAsync();

            return Ok(new GeneratePatternResponse
            {
                PatternId = pattern.Id,
                Title = patternResult.Title,
                Description = patternResult.Description,
                Difficulty = patternResult.Difficulty,
                Category = patternResult.Category,
                Materials = patternResult.Materials,
                Instructions = patternResult.Instructions,
                Notes = patternResult.Notes
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate pattern");
            return StatusCode(500, "Failed to generate pattern");
        }
    }
}
