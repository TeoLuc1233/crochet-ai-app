using CrochetAI.Api.Data;
using CrochetAI.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CrochetAI.Api.Tests.Data;

public class DatabaseSeederTests
{
    [Fact]
    public async Task SeedAsync_AddsPatternsToDatabase()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        // Act
        await DatabaseSeeder.SeedAsync(context);

        // Assert
        var patterns = await context.Patterns.ToListAsync();
        Assert.True(patterns.Count >= 5);
        Assert.Contains(patterns, p => p.Title == "Amigurumi Bunny");
        Assert.Contains(patterns, p => p.Difficulty == "Beginner");
        Assert.Contains(patterns, p => p.Difficulty == "Intermediate");
        Assert.Contains(patterns, p => p.Difficulty == "Advanced");
    }

    [Fact]
    public async Task SeedAsync_DoesNotDuplicatePatterns()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        // Act
        await DatabaseSeeder.SeedAsync(context);
        var countAfterFirstSeed = await context.Patterns.CountAsync();
        
        await DatabaseSeeder.SeedAsync(context);
        var countAfterSecondSeed = await context.Patterns.CountAsync();

        // Assert
        Assert.Equal(countAfterFirstSeed, countAfterSecondSeed);
    }
}
