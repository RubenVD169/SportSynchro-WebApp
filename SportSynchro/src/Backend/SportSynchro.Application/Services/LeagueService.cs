using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Leagues;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Services;

public sealed class LeagueService : ILeagueService
{
    private readonly ILeagueRepository _leagueRepository;
    private readonly ITeamImportService _teamImportService;
    private readonly IMatchImportService _matchImportService;

    public LeagueService(
        ILeagueRepository leagueRepository,
        ITeamImportService teamImportService,
        IMatchImportService matchImportService)
    {
        _leagueRepository = leagueRepository;
        _teamImportService = teamImportService;
        _matchImportService = matchImportService;
    }

    public async Task<IReadOnlyList<LeagueUserModel>> 
        GetAllForUserBySportIdAsync(int sportId, CancellationToken cancellationToken)
    {
        IReadOnlyList<League> leagues = await _leagueRepository.GetForUserBySportIdAsync(sportId, cancellationToken);
        return [.. leagues
            .Select(l => new LeagueUserModel(
                l.Id,
                l.Name.Value
            ))];
    }

    public async Task<IReadOnlyList<LeagueAdminModel>> 
        GetLeaguesForAdminBySportIdAsync(int sportId, CancellationToken cancellationToken)
    {
        IReadOnlyList<League> leagues = await _leagueRepository.GetBySportIdAsync(
            sportId,
            cancellationToken);
        
        return [.. leagues
            .Select(l => new LeagueAdminModel(
                l.Id,
                l.Name.Value,
                l.IsVisible
            ))];
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
        //TODO
        // if (!league.TeamsImported)
        // {
        //     await _teamImportService.ImportTeamsForLeagueAsync(
        //         league,
        //         cancellationToken);

        //     league.MarkTeamsImported();
        // }

        // if (!league.MatchesImported)
        // {
        //     DateTime? latestImportedUtc =
        //         await _matchImportService.ImportMatchesForLeagueAsync(
        //             league,
        //             cancellationToken);

        //     if (latestImportedUtc is not null)
        //     {
        //         league.MarkMatchesImportedUntil(latestImportedUtc.Value);
        //     }
        // }

        await _leagueRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}