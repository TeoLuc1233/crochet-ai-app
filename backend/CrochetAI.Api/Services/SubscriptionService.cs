using CrochetAI.Api.Data;
using CrochetAI.Api.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace CrochetAI.Api.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SubscriptionService> _logger;
    private readonly string _stripeApiKey;

    public SubscriptionService(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<SubscriptionService> logger)
    {
        _context = context;
        _logger = logger;
        _stripeApiKey = configuration["Stripe:ApiKey"] ?? "";
        if (!string.IsNullOrEmpty(_stripeApiKey))
        {
            StripeConfiguration.ApiKey = _stripeApiKey;
        }
    }

    public async Task<string> CreateCheckoutSessionAsync(string userId, string priceId)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = priceId,
                    Quantity = 1,
                },
            },
            Mode = "subscription",
            SuccessUrl = "https://localhost:3000/subscription/success?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = "https://localhost:3000/subscription/cancel",
            ClientReferenceId = userId,
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return session.Url;
    }

    public async Task<bool> CancelSubscriptionAsync(string userId)
    {
        var subscription = await GetUserSubscriptionAsync(userId);
        if (subscription == null || string.IsNullOrEmpty(subscription.StripeSubscriptionId))
        {
            return false;
        }

        try
        {
            var service = new SubscriptionService();
            await service.CancelAsync(subscription.StripeSubscriptionId);

            subscription.Status = "canceled";
            subscription.EndDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel subscription");
            return false;
        }
    }

    public async Task<bool> UpdateSubscriptionAsync(string userId, string newPriceId)
    {
        var subscription = await GetUserSubscriptionAsync(userId);
        if (subscription == null || string.IsNullOrEmpty(subscription.StripeSubscriptionId))
        {
            return false;
        }

        try
        {
            var service = new Stripe.SubscriptionService();
            var updateOptions = new SubscriptionUpdateOptions
            {
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Id = subscription.StripeSubscriptionId,
                        Price = newPriceId,
                    },
                },
            };

            await service.UpdateAsync(subscription.StripeSubscriptionId, updateOptions);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update subscription");
            return false;
        }
    }

    public async Task<Subscription?> GetUserSubscriptionAsync(string userId)
    {
        return await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "active");
    }
}
