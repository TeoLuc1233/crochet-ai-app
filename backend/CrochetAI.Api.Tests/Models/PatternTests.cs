using CrochetAI.Api.Models;

namespace CrochetAI.Api.Tests.Models;

public class PatternTests
{
    [Fact]
    public void Pattern_CanBeCreated()
    {
        // Arrange & Act
        var pattern = new Pattern
        {
            Title = "Test Pattern",
            Description = "Test Description",
            Difficulty = "Beginner",
            Category = "Amigurumi",
            Materials = "{\"yarn\":\"worsted\",\"hook\":\"5mm\"}",
            Instructions = "Row 1: Chain 10"
        };

        // Assert
        Assert.NotNull(pattern);
        Assert.Equal("Test Pattern", pattern.Title);
        Assert.Equal("Beginner", pattern.Difficulty);
        Assert.False(pattern.IsPremium);
        Assert.Equal(0, pattern.ViewCount);
    }

    [Fact]
    public void Pattern_HasDefaultValues()
    {
        // Arrange & Act
        var pattern = new Pattern();

        // Assert
        Assert.Equal(string.Empty, pattern.Title);
        Assert.Equal(string.Empty, pattern.Difficulty);
        Assert.False(pattern.IsPremium);
        Assert.Equal(0, pattern.ViewCount);
        Assert.NotNull(pattern.Projects);
    }
}
