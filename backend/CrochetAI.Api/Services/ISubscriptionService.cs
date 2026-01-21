namespace CrochetAI.Api.Services;

public interface ISubscriptionService
{
    Task<string> CreateCheckoutSessionAsync(string userId, string priceId);
    Task<bool> CancelSubscriptionAsync(string userId);
    Task<bool> UpdateSubscriptionAsync(string userId, string newPriceId);
    Task<Models.Subscription?> GetUserSubscriptionAsync(string userId);
}
