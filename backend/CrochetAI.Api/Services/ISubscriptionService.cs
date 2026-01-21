using CrochetAI.Api.Models;

namespace CrochetAI.Api.Services;

public interface ISubscriptionService
{
    Task<string> CreateCheckoutSessionAsync(string userId, string priceId);
    Task<bool> CancelSubscriptionAsync(string userId);
    Task<bool> UpdateSubscriptionAsync(string userId, string newPriceId);
    Task<Subscription?> GetUserSubscriptionAsync(string userId);
}
