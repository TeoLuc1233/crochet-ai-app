using System.Security.Claims;
using CrochetAI.Api.Controllers;
using CrochetAI.Api.DTOs;
using CrochetAI.Api.Models;
using CrochetAI.Api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CrochetAI.Api.Tests.Controllers;

public class ProjectsControllerTests
{
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly Mock<IPatternRepository> _mockPatternRepository;
    private readonly Mock<ILogger<ProjectsController>> _mockLogger;
    private readonly ProjectsController _controller;

    public ProjectsControllerTests()
    {
        _mockProjectRepository = new Mock<IProjectRepository>();
        _mockPatternRepository = new Mock<IPatternRepository>();
        _mockLogger = new Mock<ILogger<ProjectsController>>();
        
        // Setup controller with mocked context
        var context = new Mock<Microsoft.EntityFrameworkCore.DbContext>();
        _controller = new ProjectsController(
            _mockProjectRepository.Object,
            _mockPatternRepository.Object,
            new CrochetAI.Api.Data.ApplicationDbContext(new Microsoft.EntityFrameworkCore.DbContextOptions<CrochetAI.Api.Data.ApplicationDbContext>()),
            _mockLogger.Object
        );

        // Setup user claims
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "user1") };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetProjects_ReturnsUserProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new Project { Id = 1, Name = "Project 1", UserId = "user1" },
            new Project { Id = 2, Name = "Project 2", UserId = "user1" }
        };
        _mockProjectRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Project, bool>>>()))
            .ReturnsAsync(projects);

        // Act
        var result = await _controller.GetProjects();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(okResult.Value);
        Assert.Equal(2, dtos.Count());
    }

    [Fact]
    public async Task CreateProject_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateProjectRequest { Name = "New Project" };
        var project = new Project { Id = 1, Name = "New Project", UserId = "user1" };
        _mockProjectRepository.Setup(r => r.AddAsync(It.IsAny<Project>()))
            .ReturnsAsync(project);

        // Act
        var result = await _controller.CreateProject(request);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdAtResult.StatusCode);
    }
}
