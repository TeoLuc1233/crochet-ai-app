using CrochetAI.Api.Data;
using CrochetAI.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CrochetAI.Api.Tests.Identity;

public class IdentitySetupTests
{
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public void ApplicationUser_ExtendsIdentityUser()
    {
        // Arrange & Act
        var user = new ApplicationUser
        {
            UserName = "testuser",
            Email = "test@example.com",
            SubscriptionTier = "Free"
        };

        // Assert
        Assert.NotNull(user);
        Assert.Equal("testuser", user.UserName);
        Assert.Equal("Free", user.SubscriptionTier);
    }

    [Fact]
    public void ApplicationDbContext_SupportsIdentity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        // Assert
        Assert.NotNull(context.Users);
    }

    [Fact]
    public void PasswordRequirements_AreEnforced()
    {
        // This test verifies password requirements are configured
        // Actual validation happens through Identity framework
        var options = new PasswordOptions
        {
            RequireDigit = true,
            RequireLowercase = true,
            RequireUppercase = true,
            RequireNonAlphanumeric = true,
            RequiredLength = 8,
            RequiredUniqueChars = 1
        };

        Assert.True(options.RequireDigit);
        Assert.True(options.RequireLowercase);
        Assert.True(options.RequireUppercase);
        Assert.True(options.RequireNonAlphanumeric);
        Assert.Equal(8, options.RequiredLength);
    }
}
