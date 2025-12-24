using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Services;

public sealed class LeagueActivationService : ILeagueActivationService
{
    private readonly ILeagueRepository _leagueRepository;
    private readonly ITeamImportService _teamImportService;
    private readonly IMatchImportService _matchImportService;

    public LeagueActivationService(
        ILeagueRepository leagueRepository,
        ITeamImportService teamImportService,
        IMatchImportService matchImportService)
    {
        _leagueRepository = leagueRepository;
        _teamImportService = teamImportService;
        _matchImportService = matchImportService;
    }

    public async Task<bool> SetLeagueVisibilityAsync(
        int leagueId,
        bool isVisible,
        CancellationToken cancellationToken = default)
    {
        League? league =
            await _leagueRepository.GetByIdAsync(
                leagueId,
                cancellationToken);

        if (league is null)
            return false;

        league.SetVisibility(isVisible);

        if (!isVisible)
        {
            await _leagueRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        if (!league.TeamsImported)
        {
            await _teamImportService.ImportTeamsForLeagueAsync(
                league,
                cancellationToken);

            league.MarkTeamsImported();
        }

        if (!league.MatchesImported)
        {
            DateTime? latestImportedUtc =
                await _matchImportService.ImportMatchesForLeagueAsync(
                    league,
                    cancellationToken);

            if (latestImportedUtc is not null)
            {
                league.MarkMatchesImportedUntil(latestImportedUtc.Value);
            }
        }

        await _leagueRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}