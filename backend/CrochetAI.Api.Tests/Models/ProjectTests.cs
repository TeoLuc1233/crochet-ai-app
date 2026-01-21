using CrochetAI.Api.Models;

namespace CrochetAI.Api.Tests.Models;

public class ProjectTests
{
    [Fact]
    public void Project_CanBeCreated()
    {
        // Arrange & Act
        var project = new Project
        {
            UserId = "user-123",
            PatternId = 1,
            Title = "My First Project",
            Status = "InProgress",
            Progress = "{\"currentRow\":5,\"totalRows\":20}"
        };

        // Assert
        Assert.NotNull(project);
        Assert.Equal("user-123", project.UserId);
        Assert.Equal(1, project.PatternId);
        Assert.Equal("InProgress", project.Status);
    }

    [Fact]
    public void Project_HasDefaultStatus()
    {
        // Arrange & Act
        var project = new Project
        {
            UserId = "user-123",
            Title = "Test Project"
        };

        // Assert
        Assert.Equal("InProgress", project.Status);
    }
}
