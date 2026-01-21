using System.Security.Claims;
using CrochetAI.Api.Data;
using CrochetAI.Api.DTOs;
using CrochetAI.Api.Models;
using CrochetAI.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrochetAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IPatternRepository _patternRepository;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(
        IProjectRepository projectRepository,
        IPatternRepository patternRepository,
        ApplicationDbContext context,
        ILogger<ProjectsController> logger)
    {
        _projectRepository = projectRepository;
        _patternRepository = patternRepository;
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var projects = await _projectRepository.FindAsync(p => p.UserId == userId);
        var dtos = projects.Select(p => {
            var progressData = !string.IsNullOrEmpty(p.Progress) 
                ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(p.Progress) 
                : null;
            var progressPercent = progressData != null && progressData.ContainsKey("currentRow") && progressData.ContainsKey("totalRows")
                ? (int)((double)progressData["currentRow"]! / (double)progressData["totalRows"]! * 100)
                : 0;
            var notes = progressData?.ContainsKey("notes") == true ? progressData["notes"]?.ToString() : null;
            
            return new ProjectDto
            {
                Id = p.Id,
                Name = p.Title,
                Description = null,
                Status = p.Status,
                PatternId = p.PatternId,
                PatternTitle = p.Pattern?.Title,
                Progress = progressPercent,
                Notes = notes,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        });

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null || project.UserId != userId)
        {
            return NotFound();
        }

        var progressData = !string.IsNullOrEmpty(project.Progress) 
            ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(project.Progress) 
            : null;
        var progressPercent = progressData != null && progressData.ContainsKey("currentRow") && progressData.ContainsKey("totalRows")
            ? (int)((double)progressData["currentRow"]! / (double)progressData["totalRows"]! * 100)
            : 0;
        var notes = progressData?.ContainsKey("notes") == true ? progressData["notes"]?.ToString() : null;

        var dto = new ProjectDto
        {
            Id = project.Id,
            Name = project.Title,
            Description = null,
            Status = project.Status,
            PatternId = project.PatternId,
            PatternTitle = project.Pattern?.Title,
            Progress = progressPercent,
            Notes = notes,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var project = new Project
        {
            Title = request.Name,
            UserId = userId,
            PatternId = request.PatternId,
            Status = "NotStarted",
            Progress = System.Text.Json.JsonSerializer.Serialize(new { currentRow = 0, totalRows = 0, notes = request.Description ?? "" }),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _projectRepository.AddAsync(project);
        await _context.SaveChangesAsync();

        var dto = new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            PatternId = project.PatternId,
            Progress = project.Progress,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null || project.UserId != userId)
        {
            return NotFound();
        }

        var progressData = !string.IsNullOrEmpty(project.Progress) 
            ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(project.Progress) 
            : new Dictionary<string, object>();
        
        if (request.Name != null) project.Title = request.Name;
        if (request.Status != null) project.Status = request.Status;
        if (request.Progress.HasValue || request.Notes != null)
        {
            if (request.Progress.HasValue && progressData.ContainsKey("totalRows"))
            {
                progressData["currentRow"] = request.Progress.Value;
            }
            if (request.Notes != null) progressData["notes"] = request.Notes;
            project.Progress = System.Text.Json.JsonSerializer.Serialize(progressData);
        }
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project);
        await _context.SaveChangesAsync();

        var progressData = !string.IsNullOrEmpty(project.Progress) 
            ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(project.Progress) 
            : null;
        var progressPercent = progressData != null && progressData.ContainsKey("currentRow") && progressData.ContainsKey("totalRows")
            ? (int)((double)progressData["currentRow"]! / (double)progressData["totalRows"]! * 100)
            : 0;
        var notes = progressData?.ContainsKey("notes") == true ? progressData["notes"]?.ToString() : null;

        var dto = new ProjectDto
        {
            Id = project.Id,
            Name = project.Title,
            Description = null,
            Status = project.Status,
            PatternId = project.PatternId,
            PatternTitle = project.Pattern?.Title,
            Progress = progressPercent,
            Notes = notes,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null || project.UserId != userId)
        {
            return NotFound();
        }

        await _projectRepository.DeleteAsync(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
