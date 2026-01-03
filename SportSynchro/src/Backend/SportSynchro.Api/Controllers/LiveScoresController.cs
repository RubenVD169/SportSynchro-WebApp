using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportSynchro.Api.Contracts.LiveScore.Responses;

namespace SportSynchro.Api.Controllers;

[ApiController]
[Route("api/livescores")]
[Authorize (Policy = "UserRead")] 
public sealed class LiveScoresController : ControllerBase
{
    private readonly LiveScoreClient _liveScore;

    public LiveScoresController(LiveScoreClient liveScore)
    {
        _liveScore = liveScore;
    }

    [HttpGet("league/{leagueId}")]
    public async Task<IActionResult> GetByLeague(
        string leagueId,
        CancellationToken ct)
    {
        IReadOnlyList<LiveMatchResponse> matches = await _liveScore.GetLiveByLeagueAsync(leagueId, ct);
        if (matches.Count == 0)
        {
            return NotFound();
        }
        return Ok(matches);
    }
}
