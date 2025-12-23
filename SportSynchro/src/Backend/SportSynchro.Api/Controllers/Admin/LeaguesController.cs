using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportSynchro.Api.Contracts.Leagues;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Domain.Entities;
using SportSynchro.Infrastructure.Persistence;

namespace SportSynchro.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/leagues")]
public sealed class LeaguesController : ControllerBase
{
    private readonly SportSynchroDbContext _db;
    private readonly ILeagueActivationService _activationService;

    public LeaguesController(
        SportSynchroDbContext db,
        ILeagueActivationService activationService)
    {
        _db = db;
        _activationService = activationService;
    }

    [HttpPatch("{leagueId:int}/visibility")]
    public async Task<IActionResult> SetVisibility(
        int leagueId,
        SetLeagueVisibilityRequest request,
        CancellationToken cancellationToken)
    {
        League? league =
            await _db.Leagues
                .FirstOrDefaultAsync(l => l.Id == leagueId, cancellationToken);

        if (league is null)
            return NotFound();

        await _activationService.SetLeagueVisibilityAsync(
            league,
            request.IsVisible,
            cancellationToken);

        return NoContent();
    }
}
