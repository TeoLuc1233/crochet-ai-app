using CrochetAI.Api.Models;
using CrochetAI.Api.Repositories;

namespace CrochetAI.Api.Tests.Repositories;

public class RepositoryInterfaceTests
{
    [Fact]
    public void IPatternRepository_ExtendsIRepository()
    {
        // This test verifies the interface structure
        Assert.True(typeof(IPatternRepository).IsInterface);
        Assert.True(typeof(IRepository<Pattern>).IsAssignableFrom(typeof(IPatternRepository)));
    }

    [Fact]
    public void IProjectRepository_ExtendsIRepository()
    {
        Assert.True(typeof(IProjectRepository).IsInterface);
        Assert.True(typeof(IRepository<Project>).IsAssignableFrom(typeof(IProjectRepository)));
    }

    [Fact]
    public void ISubscriptionRepository_ExtendsIRepository()
    {
        Assert.True(typeof(ISubscriptionRepository).IsInterface);
        Assert.True(typeof(IRepository<Subscription>).IsAssignableFrom(typeof(ISubscriptionRepository)));
    }

    [Fact]
    public void IAIGenerationRepository_ExtendsIRepository()
    {
        Assert.True(typeof(IAIGenerationRepository).IsInterface);
        Assert.True(typeof(IRepository<AIGeneration>).IsAssignableFrom(typeof(IAIGenerationRepository)));
    }

    [Fact]
    public void IRefreshTokenRepository_ExtendsIRepository()
    {
        Assert.True(typeof(IRefreshTokenRepository).IsInterface);
        Assert.True(typeof(IRepository<RefreshToken>).IsAssignableFrom(typeof(IRefreshTokenRepository)));
    }

    [Fact]
    public void IAuditLogRepository_ExtendsIRepository()
    {
        Assert.True(typeof(IAuditLogRepository).IsInterface);
        Assert.True(typeof(IRepository<AuditLog>).IsAssignableFrom(typeof(IAuditLogRepository)));
    }
}
