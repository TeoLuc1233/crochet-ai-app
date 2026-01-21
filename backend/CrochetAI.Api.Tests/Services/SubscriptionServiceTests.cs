using CrochetAI.Api.Data;
using CrochetAI.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace CrochetAI.Api.Tests.Services;

public class SubscriptionServiceTests
{
    [Fact]
    public void SubscriptionService_WithMissingApiKey_DoesNotThrow()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        var context = new ApplicationDbContext(options);
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c["Stripe:ApiKey"]).Returns((string?)null);
        var logger = new Mock<ILogger<SubscriptionService>>();

        // Act & Assert - Should not throw
        var service = new SubscriptionService(context, configuration.Object, logger.Object);
        Assert.NotNull(service);
    }
}
