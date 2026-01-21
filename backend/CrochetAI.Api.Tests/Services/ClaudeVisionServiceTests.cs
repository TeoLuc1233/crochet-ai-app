using CrochetAI.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace CrochetAI.Api.Tests.Services;

public class ClaudeVisionServiceTests
{
    [Fact]
    public void ClaudeVisionService_WithMissingApiKey_ThrowsException()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c["Anthropic:ApiKey"]).Returns((string?)null);
        var logger = new Mock<ILogger<ClaudeVisionService>>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new ClaudeVisionService(configuration.Object, logger.Object));
    }
}
