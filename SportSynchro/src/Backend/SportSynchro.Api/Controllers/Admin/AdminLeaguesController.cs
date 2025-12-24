using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportSynchro.Api.Contracts.Leagues;
using SportSynchro.Application.Interfaces.Services;

namespace SportSynchro.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/leagues")]
public sealed class AdminLeaguesController : ControllerBase
{
    private readonly ILeagueActivationService _activationService;

    public AdminLeaguesController(
        ILeagueActivationService activationService)
    {
        _activationService = activationService;
    }

    [Authorize(Policy = "AdminRead")]
    [HttpPatch("{leagueId:int}/visibility")]
    public async Task<IActionResult> SetVisibility(
        int leagueId,
        [FromBody] SetLeagueVisibilityRequest request,
        CancellationToken cancellationToken)
    {
        bool updated =
            await _activationService.SetLeagueVisibilityAsync(
                leagueId,
                request.IsVisible,
                cancellationToken);

        if (!updated)
            return NotFound();

        return NoContent();
    }
}
