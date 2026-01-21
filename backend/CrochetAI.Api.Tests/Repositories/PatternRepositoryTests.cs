using CrochetAI.Api.Data;
using CrochetAI.Api.Models;
using CrochetAI.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CrochetAI.Api.Tests.Repositories;

public class PatternRepositoryTests
{
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetByDifficultyAsync_ReturnsMatchingPatterns()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PatternRepository(context);

        var pattern1 = new Pattern { Title = "Test 1", Difficulty = "Beginner", Category = "Test", Materials = "{}", Instructions = "Test" };
        var pattern2 = new Pattern { Title = "Test 2", Difficulty = "Intermediate", Category = "Test", Materials = "{}", Instructions = "Test" };

        context.Patterns.AddRange(pattern1, pattern2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByDifficultyAsync("Beginner");

        // Assert
        Assert.Single(result);
        Assert.Equal("Test 1", result.First().Title);
    }

    [Fact]
    public async Task SearchAsync_FindsPatternsByTitle()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PatternRepository(context);

        var pattern = new Pattern { Title = "Amigurumi Bunny", Difficulty = "Beginner", Category = "Test", Materials = "{}", Instructions = "Test" };
        context.Patterns.Add(pattern);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("bunny");

        // Assert
        Assert.Single(result);
        Assert.Equal("Amigurumi Bunny", result.First().Title);
    }

    [Fact]
    public async Task IncrementViewCountAsync_IncrementsCount()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new PatternRepository(context);

        var pattern = new Pattern { Title = "Test", Difficulty = "Beginner", Category = "Test", Materials = "{}", Instructions = "Test", ViewCount = 5 };
        context.Patterns.Add(pattern);
        await context.SaveChangesAsync();

        // Act
        await repository.IncrementViewCountAsync(pattern.Id);
        
        // Assert
        var updated = await context.Patterns.FindAsync(pattern.Id);
        Assert.Equal(6, updated!.ViewCount);
    }
}
