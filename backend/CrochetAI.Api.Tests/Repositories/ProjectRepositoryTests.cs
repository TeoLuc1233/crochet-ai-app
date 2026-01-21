using CrochetAI.Api.Data;
using CrochetAI.Api.Models;
using CrochetAI.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CrochetAI.Api.Tests.Repositories;

public class ProjectRepositoryTests
{
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsUserProjects()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ProjectRepository(context);

        var project1 = new Project { UserId = "user-1", Title = "Project 1", Status = "InProgress" };
        var project2 = new Project { UserId = "user-2", Title = "Project 2", Status = "InProgress" };

        context.Projects.AddRange(project1, project2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByUserIdAsync("user-1");

        // Assert
        Assert.Single(result);
        Assert.Equal("Project 1", result.First().Title);
    }

    [Fact]
    public async Task GetByIdAndUserIdAsync_ReturnsCorrectProject()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ProjectRepository(context);

        var project = new Project { UserId = "user-1", Title = "Project 1", Status = "InProgress" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAndUserIdAsync(project.Id, "user-1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Project 1", result.Title);
    }

    [Fact]
    public async Task GetByIdAndUserIdAsync_ReturnsNullForWrongUser()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ProjectRepository(context);

        var project = new Project { UserId = "user-1", Title = "Project 1", Status = "InProgress" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAndUserIdAsync(project.Id, "user-2");

        // Assert
        Assert.Null(result);
    }
}
