using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrochetAI.Api.Models;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string TokenHash { get; set; } = string.Empty; // SHA256 hash

    [Required]
    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? RevokedAt { get; set; }
}
