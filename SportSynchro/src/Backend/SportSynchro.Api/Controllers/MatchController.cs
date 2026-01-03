using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportSynchro.Api.Contracts.Match.Responses;
using SportSynchro.Api.Mapping.Matches;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Matches;

namespace SportSynchro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MatchController : ControllerBase
{
    private readonly IMatchService _matchService;
    public MatchController(IMatchService matchService)
    {
        _matchService = matchService;
    }

    [Authorize(policy: "UserRead")]
    [HttpGet ("{leagueId:int}")]
    public async Task<IActionResult> GetRecentMatches(
        [FromRoute] int leagueId, 
        CancellationToken cancellationToken
        )
    {
        IReadOnlyList<MatchModel> matches = 
            await _matchService.GetRecentMatchesByLeagueIdAsync(leagueId, cancellationToken);

        IReadOnlyList<MatchResponseContract> response = matches.ToResponseContractList();
        if (response.Count == 0)
            return NotFound();
        
        return Ok(response);
    }

    [Authorize(policy: "UserRead")]
    [HttpGet("schedule/{leagueId:int}")]
    public async Task<IActionResult> GetScheduledMatches(
        [FromRoute] int leagueId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<MatchModel> matches =
            await _matchService.GetScheduledMatchesByLeagueIdAsync(leagueId, cancellationToken);

        IReadOnlyList<MatchResponseContract> response = matches.ToResponseContractList();
        if (response.Count == 0)
            return NotFound();

        return Ok(response);
    }
}