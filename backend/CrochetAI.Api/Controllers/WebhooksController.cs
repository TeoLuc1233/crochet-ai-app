using CrochetAI.Api.Data;
using CrochetAI.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace CrochetAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WebhooksController> _logger;
    private readonly string _webhookSecret;

    public WebhooksController(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<WebhooksController> logger)
    {
        _context = context;
        _logger = logger;
        _webhookSecret = configuration["Stripe:WebhookSecret"] ?? "";
    }

    [HttpPost("stripe")]
    public async Task<IActionResult> HandleStripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _webhookSecret
            );

            switch (stripeEvent.Type)
            {
                case "checkout.session.completed":
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    if (session != null && session.Mode == "subscription" && session.ClientReferenceId != null)
                    {
                        await HandleSubscriptionCreated(session);
                    }
                    break;

                case "customer.subscription.updated":
                    var subscription = stripeEvent.Data.Object as Stripe.Subscription;
                    if (subscription != null)
                    {
                        await HandleSubscriptionUpdated(subscription);
                    }
                    break;

                case "customer.subscription.deleted":
                    var deletedSubscription = stripeEvent.Data.Object as Stripe.Subscription;
                    if (deletedSubscription != null)
                    {
                        await HandleSubscriptionDeleted(deletedSubscription);
                    }
                    break;
            }

            return Ok();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe webhook error");
            return BadRequest();
        }
    }

    private async Task HandleSubscriptionCreated(Stripe.Checkout.Session session)
    {
        var userId = session.ClientReferenceId;
        var subscriptionService = new Stripe.SubscriptionService();
        var subscription = await subscriptionService.GetAsync(session.SubscriptionId);

        var dbSubscription = new Models.Subscription
        {
            UserId = userId!,
            StripeSubscriptionId = subscription.Id,
            StripeCustomerId = subscription.CustomerId ?? "",
            Tier = DetermineTierFromPlanId(subscription.Items.Data[0].Price.Id),
            Status = subscription.Status,
            CurrentPeriodStart = DateTimeOffset.FromUnixTimeSeconds(subscription.CurrentPeriodStart).DateTime,
            CurrentPeriodEnd = DateTimeOffset.FromUnixTimeSeconds(subscription.CurrentPeriodEnd).DateTime,
            CreatedAt = DateTime.UtcNow
        };

        _context.Subscriptions.Add(dbSubscription);

        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.SubscriptionTier = dbSubscription.Tier;
            await _context.SaveChangesAsync();
        }
    }

    private async Task HandleSubscriptionUpdated(Stripe.Subscription subscription)
    {
        var dbSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == subscription.Id);

        if (dbSubscription != null)
        {
            dbSubscription.Status = subscription.Status;
            dbSubscription.CurrentPeriodEnd = DateTimeOffset.FromUnixTimeSeconds(subscription.CurrentPeriodEnd).DateTime;
            dbSubscription.Tier = DetermineTierFromPlanId(subscription.Items.Data[0].Price.Id);

            var user = await _context.Users.FindAsync(dbSubscription.UserId);
            if (user != null)
            {
                user.SubscriptionTier = dbSubscription.Tier;
            }

            await _context.SaveChangesAsync();
        }
    }

    private async Task HandleSubscriptionDeleted(Stripe.Subscription subscription)
    {
        var dbSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == subscription.Id);

        if (dbSubscription != null)
        {
            dbSubscription.Status = "canceled";
            dbSubscription.CanceledAt = DateTime.UtcNow;

            var user = await _context.Users.FindAsync(dbSubscription.UserId);
            if (user != null)
            {
                user.SubscriptionTier = "Free";
            }

            await _context.SaveChangesAsync();
        }
    }

    private string DetermineTierFromPlanId(string planId)
    {
        // This should match your Stripe price IDs
        if (planId.Contains("premium", StringComparison.OrdinalIgnoreCase))
            return "Premium";
        if (planId.Contains("pro", StringComparison.OrdinalIgnoreCase))
            return "Pro";
        return "Free";
    }
}
