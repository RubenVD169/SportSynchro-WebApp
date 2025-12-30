using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        string json = await _liveScore.GetLiveByLeagueAsync(leagueId, ct);
        return Content(json, "application/json");
    }
}
