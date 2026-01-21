using CrochetAI.Api.Models;

namespace CrochetAI.Api.Repositories;

public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<Subscription?> GetByUserIdAsync(string userId);
    Task<Subscription?> GetByStripeSubscriptionIdAsync(string stripeSubscriptionId);
    Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync();
}
