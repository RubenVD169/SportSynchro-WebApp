using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportSynchro.Api.Contracts.Leagues.Requests;
using SportSynchro.Api.Contracts.Leagues.Responses;
using SportSynchro.Api.Mapping.Leagues;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Leagues;

namespace SportSynchro.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/leagues")]
public sealed class AdminLeaguesController : ControllerBase
{
    private readonly ILeagueService _leagueService;

    public AdminLeaguesController(
        ILeagueService leagueService)
    {
        _leagueService = leagueService;
    }

    [Authorize(Policy = "AdminRead")]
    [HttpPatch("{leagueId:int}/visibility")]
    public async Task<IActionResult> SetVisibility(
        int leagueId,
        [FromBody] SetLeagueVisibilityRequest request,
        CancellationToken cancellationToken)
    {
        bool updated =
            await _leagueService.SetLeagueVisibilityAsync(
                leagueId,
                request.IsVisible,
                cancellationToken);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [Authorize(Policy = "AdminRead")]
    [HttpGet("{sportId:int}")]
    public async Task<IActionResult> GetAllLeaguesBySportId(
        [FromRoute] int sportId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<LeagueAdminModel> models =
            await _leagueService.GetLeaguesForAdminBySportIdAsync(
                sportId,
                cancellationToken);

        IReadOnlyList<LeagueAdminResponse> response = models.ToAdminResponses();

        return response.Count switch
        {
            0 => NotFound(),
            > 0 => Ok(response),
            _ => StatusCode(500),
        };
    }
}
