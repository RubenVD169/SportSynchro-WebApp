using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SportSynchro.Api.Options.ExternalOptions;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Subscriptions;
using Stripe;
using Stripe.Checkout;

namespace SportSynchro.Api.Controllers.Stripe;

[ApiController]
[Route("api/[controller]")]
public sealed class StripeController : ControllerBase
{
    private readonly StripeOptions _stripeOptions;
    private readonly ISubscriptionService _subscriptionService;

    public StripeController(
        IOptions<StripeOptions> stripeOptions,
        ISubscriptionService subscriptionService)
    {
        _stripeOptions = stripeOptions.Value;
        _subscriptionService = subscriptionService;
    }

    [Authorize]
    [HttpPost("checkout")]
    public IActionResult CreateCheckout()
    {
        string? aspUserId =
            User.FindFirstValue("sub") ??
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(aspUserId))
            return Unauthorized();

        StripeConfiguration.ApiKey = _stripeOptions.SecretKey;

        SessionCreateOptions options = new()
        {
            Mode = "payment",
            SuccessUrl = _stripeOptions.SuccessUrl,
            CancelUrl = _stripeOptions.CancelUrl,
            ClientReferenceId = aspUserId,

            LineItems =
            [
                new SessionLineItemOptions
                {
                    Price = "price_1SlA3mARmd8FyWUhdMCZrEFM",
                    Quantity = 1
                }
            ]
        };

        SessionService service = new();
        Session? session = service.Create(options);

        return Ok(new
        {
            url = session.Url,
            sessionId = session.Id
        });
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        string json = await new StreamReader(Request.Body).ReadToEndAsync();

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _stripeOptions.WebhookSecret
            );
        }
        catch
        {
            return BadRequest();
        }

        if (stripeEvent.Type != "checkout.session.completed" 
            || stripeEvent.Data.Object is not Session session 
            || string.IsNullOrWhiteSpace(session.ClientReferenceId))
            return Ok();


        ActivateSubscriptionModel model = new(
            session.ClientReferenceId,
            session.Id
        );

        await _subscriptionService.ActivatePremiumAsync(model);

        return Ok();
    }
}
