using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrochetAI.Api.Models;

public class AIGeneration
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string ImageHash { get; set; } = string.Empty; // SHA256 for caching

    [Required]
    [Column(TypeName = "jsonb")]
    public string AnalysisResult { get; set; } = string.Empty; // JSON

    [Required]
    public string GeneratedPattern { get; set; } = string.Empty;

    public bool CachedResponse { get; set; } = false;

    public int GenerationTimeMs { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
