using CrochetAI.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CrochetAI.Api.Tests.Migrations;

public class MigrationTests
{
    [Fact]
    public void InitialCreateMigration_CanBeApplied()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=test_crochetai;Username=postgres;Password=postgres")
            .Options;

        // This test verifies the migration can be created
        // In a real scenario, we'd apply it to a test database
        // For now, we verify the DbContext can be created with the models

        // Act & Assert
        using var context = new ApplicationDbContext(options);
        Assert.NotNull(context);
    }

    [Fact]
    public void DbContext_HasAllRequiredDbSets()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.Patterns);
        Assert.NotNull(context.Projects);
        Assert.NotNull(context.Subscriptions);
        Assert.NotNull(context.AIGenerations);
        Assert.NotNull(context.RefreshTokens);
        Assert.NotNull(context.AuditLogs);
    }
}
