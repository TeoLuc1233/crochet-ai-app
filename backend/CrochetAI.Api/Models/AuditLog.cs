using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrochetAI.Api.Models;

public class AuditLog
{
    [Key]
    public long Id { get; set; }

    [MaxLength(255)]
    public string? UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [MaxLength(45)]
    public string IpAddress { get; set; } = string.Empty;

    public string? UserAgent { get; set; }

    [Column(TypeName = "jsonb")]
    public string? Details { get; set; } // JSON

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
