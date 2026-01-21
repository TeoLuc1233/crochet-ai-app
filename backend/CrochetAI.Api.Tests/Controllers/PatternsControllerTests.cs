using System.Net;
using System.Net.Http.Json;
using CrochetAI.Api.Controllers;
using CrochetAI.Api.DTOs;
using CrochetAI.Api.Models;
using CrochetAI.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CrochetAI.Api.Tests.Controllers;

public class PatternsControllerTests
{
    private readonly Mock<IPatternRepository> _mockRepository;
    private readonly Mock<ILogger<PatternsController>> _mockLogger;
    private readonly PatternsController _controller;

    public PatternsControllerTests()
    {
        _mockRepository = new Mock<IPatternRepository>();
        _mockLogger = new Mock<ILogger<PatternsController>>();
        _controller = new PatternsController(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetPatterns_ReturnsPagedResults()
    {
        // Arrange
        var patterns = new List<Pattern>
        {
            new Pattern { Id = 1, Title = "Pattern 1", Difficulty = "Beginner", Category = "Amigurumi", IsPremium = false },
            new Pattern { Id = 2, Title = "Pattern 2", Difficulty = "Intermediate", Category = "Clothing", IsPremium = false }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(patterns);

        // Act
        var result = await _controller.GetPatterns(page: 1, pageSize: 20);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PatternListResponse>(okResult.Value);
        Assert.Equal(2, response.Patterns.Count);
        Assert.Equal(2, response.TotalCount);
    }

    [Fact]
    public async Task GetPattern_WithValidId_ReturnsPattern()
    {
        // Arrange
        var pattern = new Pattern
        {
            Id = 1,
            Title = "Test Pattern",
            Description = "Test Description",
            Difficulty = "Beginner",
            Category = "Amigurumi",
            Materials = "{\"yarn\": \"cotton\"}",
            Instructions = "Row 1: ...",
            IsPremium = false
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(pattern);

        // Act
        var result = await _controller.GetPattern(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<PatternDto>(okResult.Value);
        Assert.Equal("Test Pattern", dto.Title);
    }

    [Fact]
    public async Task GetPattern_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Pattern?)null);

        // Act
        var result = await _controller.GetPattern(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task SearchPatterns_WithQuery_ReturnsFilteredResults()
    {
        // Arrange
        var patterns = new List<Pattern>
        {
            new Pattern { Id = 1, Title = "Amigurumi Bear", Difficulty = "Beginner", Category = "Amigurumi", IsPremium = false },
            new Pattern { Id = 2, Title = "Scarf Pattern", Difficulty = "Intermediate", Category = "Clothing", IsPremium = false }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(patterns);

        // Act
        var result = await _controller.SearchPatterns(query: "Bear");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PatternListResponse>(okResult.Value);
        Assert.Single(response.Patterns);
        Assert.Contains("Bear", response.Patterns[0].Title);
    }
}
