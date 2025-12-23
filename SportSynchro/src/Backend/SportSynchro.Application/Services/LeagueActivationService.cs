using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Domain.Entities;
using SportSynchro.Infrastructure.Persistence;

namespace SportSynchro.Application.Services;

public sealed class LeagueActivationService : ILeagueActivationService
{
    private readonly ITeamImportService _teamImportService;
    private readonly IMatchImportService _matchImportService;
    private readonly SportSynchroDbContext _db;

    public LeagueActivationService(
        ITeamImportService teamImportService,
        IMatchImportService matchImportService,
        SportSynchroDbContext db)
    {
        _teamImportService = teamImportService;
        _matchImportService = matchImportService;
        _db = db;
    }

    public async Task SetLeagueVisibilityAsync(
        League league,
        bool isVisible,
        CancellationToken cancellationToken = default)
    {
        // Change visibility domain logic
        league.SetVisibility(isVisible);

        if (!isVisible)
        {
            await _db.SaveChangesAsync(cancellationToken);
            return;
        }

        // Lazyloads teams first 
        if (!league.TeamsImported)
        {
            await _teamImportService.ImportTeamsForLeagueAsync(
                league,
                cancellationToken);

            league.MarkTeamsImported();
        }

        // Lazyloads matches next 
        if (!league.MatchesImported)
        {
            DateTime? latestImportedUtc = 
                await _matchImportService.ImportMatchesForLeagueAsync(league, cancellationToken);

            if (latestImportedUtc is not null)
            {
                league.MarkMatchesImportedUntil(latestImportedUtc.Value);
            }
        }

        // Persist state
        await _db.SaveChangesAsync(cancellationToken);
    }
}
