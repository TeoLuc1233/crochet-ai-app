using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrochetAI.Api.Models;

public class Project
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string UserId { get; set; } = string.Empty;

    public int? PatternId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "InProgress"; // NotStarted, InProgress, Completed

    [Column(TypeName = "jsonb")]
    public string? Progress { get; set; } // JSON: { "currentRow": 10, "totalRows": 50, "notes": "..." }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("PatternId")]
    public Pattern? Pattern { get; set; }
}
