using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrochetAI.Api.Models;

public class Subscription
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string StripeCustomerId { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? StripeSubscriptionId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Tier { get; set; } = string.Empty; // Free, Premium, Pro

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = string.Empty; // Active, Canceled, PastDue

    [Required]
    public DateTime CurrentPeriodStart { get; set; }

    [Required]
    public DateTime CurrentPeriodEnd { get; set; }

    public DateTime? CanceledAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
