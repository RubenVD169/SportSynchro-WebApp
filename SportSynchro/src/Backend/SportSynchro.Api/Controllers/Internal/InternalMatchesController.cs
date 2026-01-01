using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportSynchro.Api.Contracts.LiveScore.Requests;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Matches;

namespace SportSynchro.Api.Controllers.Internal;

[ApiController]
[Route("internal/matches")]
public sealed class InternalMatchesController : ControllerBase
{
    private readonly IMatchFinalizationService _service;

    public InternalMatchesController(IMatchFinalizationService service)
    {
        _service = service;
    }

    [HttpPost("finished")]
    [Authorize(Policy = "LiveScoreInternal")]
    public async Task<IActionResult> Finished(
        [FromBody] MatchFinishedRequest request,
        CancellationToken ct)
    {
        MatchFinishedModel model = new(
            request.EventId,
            request.HomeScore,
            request.AwayScore
        );

        await _service.HandleFinishedAsync(model, ct);
        return Ok();
    }
}