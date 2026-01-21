using CrochetAI.Api.Data;
using CrochetAI.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CrochetAI.Api.Tests.Data;

public class ApplicationDbContextTests
{
    [Fact]
    public void ApplicationDbContext_CanBeCreated()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(context.Patterns);
        Assert.NotNull(context.Projects);
        Assert.NotNull(context.Subscriptions);
        Assert.NotNull(context.AIGenerations);
        Assert.NotNull(context.RefreshTokens);
        Assert.NotNull(context.AuditLogs);
    }

    [Fact]
    public void ApplicationDbContext_CanSavePattern()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        var pattern = new Pattern
        {
            Title = "Test Pattern",
            Difficulty = "Beginner",
            Category = "Amigurumi",
            Materials = "{}",
            Instructions = "Test instructions"
        };

        // Act
        context.Patterns.Add(pattern);
        context.SaveChanges();

        // Assert
        var savedPattern = context.Patterns.FirstOrDefault(p => p.Id == pattern.Id);
        Assert.NotNull(savedPattern);
        Assert.Equal("Test Pattern", savedPattern.Title);
    }

    [Fact]
    public void ApplicationDbContext_CanSaveProject()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        var project = new Project
        {
            UserId = "user-123",
            Title = "Test Project",
            Status = "InProgress"
        };

        // Act
        context.Projects.Add(project);
        context.SaveChanges();

        // Assert
        var savedProject = context.Projects.FirstOrDefault(p => p.Id == project.Id);
        Assert.NotNull(savedProject);
        Assert.Equal("user-123", savedProject.UserId);
    }
}
