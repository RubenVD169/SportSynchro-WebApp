using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Domain.Entities;
using SportSynchro.Infrastructure.Persistence;

namespace SportSynchro.Application.Leagues;

public sealed class LeagueActivationService : ILeagueActivationService
{
    private readonly ITeamImportService _teamImportService;
    private readonly SportSynchroDbContext _db;

    public LeagueActivationService(
        ITeamImportService teamImportService,
        SportSynchroDbContext db)
    {
        _teamImportService = teamImportService;
        _db = db;
    }

    public async Task SetLeagueVisibilityAsync(
        League league,
        bool isVisible,
        CancellationToken cancellationToken = default)
    {
        // Change visibility domain logic
        league.SetVisibility(isVisible);

        // Lazy import teams on activation
        if (isVisible && !league.TeamsImported)
        {
            await _teamImportService.ImportTeamsForLeagueAsync(
                league,
                cancellationToken);

            league.MarkTeamsImported();
        }

        // Persist state
        await _db.SaveChangesAsync(cancellationToken);
    }
}
