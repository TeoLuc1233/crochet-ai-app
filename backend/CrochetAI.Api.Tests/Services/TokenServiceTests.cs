using CrochetAI.Api.Data;
using CrochetAI.Api.Models;
using CrochetAI.Api.Repositories;
using CrochetAI.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CrochetAI.Api.Tests.Services;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;

    public TokenServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("test-key-minimum-32-characters-long-for-security");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("CrochetAI");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("CrochetAI");
        _mockConfiguration.Setup(c => c["Jwt:AccessTokenLifetimeMinutes"]).Returns("15");
        _mockConfiguration.Setup(c => c["Jwt:RefreshTokenLifetimeDays"]).Returns("7");

        var store = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_ReturnsValidToken()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user-123",
            UserName = "testuser",
            Email = "test@example.com",
            SubscriptionTier = "Free"
        };

        _mockUserManager.Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        var service = new TokenService(
            _mockConfiguration.Object,
            _mockUserManager.Object,
            _mockRefreshTokenRepository.Object);

        // Act
        var token = await service.GenerateAccessTokenAsync(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task GenerateRefreshTokenAsync_CreatesTokenInRepository()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-123" };
        _mockRefreshTokenRepository.Setup(r => r.AddAsync(It.IsAny<RefreshToken>()))
            .ReturnsAsync(new RefreshToken());

        var service = new TokenService(
            _mockConfiguration.Object,
            _mockUserManager.Object,
            _mockRefreshTokenRepository.Object);

        // Act
        var refreshToken = await service.GenerateRefreshTokenAsync(user);

        // Assert
        Assert.NotNull(refreshToken);
        Assert.NotEmpty(refreshToken);
        _mockRefreshTokenRepository.Verify(r => r.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
    }
}
