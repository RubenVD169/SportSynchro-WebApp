using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportSynchro.Application.Interfaces.Services;

namespace SportSynchro.Api.Controllers;

[ApiController]
[Route("api/subscription")]
[Authorize]
public sealed class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [Authorize(Policy = "UserRead")]
    [HttpGet("me")]
    public async Task<IActionResult> GetMySubscriptionStatus()
    {
        string? aspUserId =
            User.FindFirstValue("sub") ??
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(aspUserId))
            return Unauthorized();

        bool isPremium = await _subscriptionService.IsUserPremiumAsync(aspUserId);

        return Ok(isPremium);
    }
}
