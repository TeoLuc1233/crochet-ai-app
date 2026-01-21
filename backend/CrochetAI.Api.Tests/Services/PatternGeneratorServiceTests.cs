using CrochetAI.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace CrochetAI.Api.Tests.Services;

public class PatternGeneratorServiceTests
{
    [Fact]
    public void PatternGeneratorService_WithMissingApiKey_ThrowsException()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c["Anthropic:ApiKey"]).Returns((string?)null);
        var logger = new Mock<ILogger<PatternGeneratorService>>();
        var redisService = new Mock<IRedisService>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            new PatternGeneratorService(configuration.Object, logger.Object, redisService.Object));
    }
}
