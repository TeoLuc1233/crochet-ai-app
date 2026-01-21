using CrochetAI.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CrochetAI.Api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Pattern> Patterns { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<AIGeneration> AIGenerations { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Pattern configuration
        modelBuilder.Entity<Pattern>(entity =>
        {
            entity.HasIndex(p => p.Difficulty)
                .HasDatabaseName("idx_difficulty");
            entity.HasIndex(p => p.Category)
                .HasDatabaseName("idx_category");
            entity.HasIndex(p => p.IsPremium)
                .HasDatabaseName("idx_premium");
        });

        // Project configuration
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasIndex(p => new { p.UserId, p.Status })
                .HasDatabaseName("idx_user_status");
            entity.HasOne(p => p.Pattern)
                .WithMany(pat => pat.Projects)
                .HasForeignKey(p => p.PatternId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // AIGeneration configuration
        modelBuilder.Entity<AIGeneration>(entity =>
        {
            entity.HasIndex(a => a.ImageHash)
                .HasDatabaseName("idx_image_hash");
            entity.HasIndex(a => new { a.UserId, a.CreatedAt })
                .HasDatabaseName("idx_user_month");
        });

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(r => r.TokenHash)
                .IsUnique()
                .HasDatabaseName("idx_token_hash");
            entity.HasIndex(r => new { r.UserId, r.ExpiresAt })
                .HasDatabaseName("idx_user_active");
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasIndex(a => new { a.UserId, a.Action })
                .HasDatabaseName("idx_user_action");
            entity.HasIndex(a => a.Timestamp)
                .HasDatabaseName("idx_timestamp");
        });
    }
}
