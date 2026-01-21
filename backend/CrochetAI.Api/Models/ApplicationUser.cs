using Microsoft.AspNetCore.Identity;

namespace CrochetAI.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string SubscriptionTier { get; set; } = "Free";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
