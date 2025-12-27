using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportSynchro.Api.Contracts.Leagues.Responses;
using SportSynchro.Api.Mapping.Leagues;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Leagues;

namespace SportSynchro.Api.Controllers.User;
[ApiController]
[Route("api/user/leagues")]
public sealed class UserLeagueController : ControllerBase
{
    private readonly ILeagueService _leagueService;

    public UserLeagueController(ILeagueService leagueService)
    {
        _leagueService = leagueService;
    }

    [Authorize(policy: "UserRead")]
    [HttpGet("{sportId:int}")]
    public async Task<IActionResult> GetAllForUser(
        [FromRoute] int sportId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<LeagueUserModel> models = await _leagueService
            .GetAllForUserBySportIdAsync(sportId, cancellationToken);
    
        IReadOnlyList<LeagueUserResponse> response = models.ToUserResponses();

        if (response.Count == 0)
            return NotFound();
        return Ok(response);
    }
}