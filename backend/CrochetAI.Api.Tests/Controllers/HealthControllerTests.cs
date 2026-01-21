using CrochetAI.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CrochetAI.Api.Tests.Controllers;

public class HealthControllerTests
{
    [Fact]
    public void Get_ReturnsOkResult()
    {
        // Arrange
        var controller = new HealthController();

        // Act
        var result = controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void Get_ReturnsStatusHealthy()
    {
        // Arrange
        var controller = new HealthController();

        // Act
        var result = controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var json = JsonSerializer.Serialize(okResult.Value);
        var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        Assert.NotNull(response);
        Assert.True(response!.ContainsKey("status"));
        Assert.Equal("Healthy", response["status"]!.ToString());
    }
}
