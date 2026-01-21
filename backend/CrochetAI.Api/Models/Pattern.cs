using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrochetAI.Api.Models;

public class Pattern
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [MaxLength(20)]
    public string Difficulty { get; set; } = string.Empty; // Beginner, Intermediate, Advanced

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // Amigurumi, Clothing, Home, etc.

    [Required]
    [Column(TypeName = "jsonb")]
    public string Materials { get; set; } = string.Empty; // JSON: { "yarn": "...", "hook": "...", "other": [...] }

    [Required]
    public string Instructions { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public bool IsPremium { get; set; } = false;

    public int ViewCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
