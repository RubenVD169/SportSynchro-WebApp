using SportSynchro.Application.Interfaces.External;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Leagues;
using SportSynchro.Domain.Entities;
using SportSynchro.Domain.ValueObjects;

namespace SportSynchro.Application.Services;

public sealed class LeagueService : ILeagueService
{
    private readonly ILeagueRepository _leagueRepository;
    private readonly ISeasonRepository _seasonRepository;
    private readonly ITeamImportService _teamImportService;
    private readonly IMatchImportService _matchImportService;
    private readonly ITheSportsDbRepository _sportsDbRepository;

    public LeagueService(
        ILeagueRepository leagueRepository,
        ISeasonRepository seasonRepository,
        ITeamImportService teamImportService,
        IMatchImportService matchImportService,
        ITheSportsDbRepository sportsDbRepository)
    {
        _leagueRepository = leagueRepository;
        _seasonRepository = seasonRepository;
        _teamImportService = teamImportService;
        _matchImportService = matchImportService;
        _sportsDbRepository = sportsDbRepository;
    }

    public async Task<IReadOnlyList<LeagueUserModel>>
        GetAllForUserBySportIdAsync(
            int sportId,
            CancellationToken cancellationToken)
    {
        IReadOnlyList<League> leagues =
            await _leagueRepository.GetForUserBySportIdAsync(
                sportId,
                cancellationToken);

        return [.. leagues
            .Select(l => new LeagueUserModel(
                l.Id,
                l.Name.Value))];
    }

    public async Task<IReadOnlyList<LeagueAdminModel>>
        GetLeaguesForAdminBySportIdAsync(
            int sportId,
            CancellationToken cancellationToken)
    {
        IReadOnlyList<League> leagues =
            await _leagueRepository.GetBySportIdAsync(
                sportId,
                cancellationToken);

        return [.. leagues
            .Select(l => new LeagueAdminModel(
                l.Id,
                l.Name.Value,
                l.IsVisible))];
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

        IReadOnlyList<string> seasons =
            await _sportsDbRepository.GetSeasonsByLeagueAsync(
                league.ExternalId,
                cancellationToken);

        string apiSeasonKey =
            seasons.Count == 0
                ? "default"
                : seasons[^1]; // last season 

        Season? currentSeason =
            await _seasonRepository.GetCurrentForLeagueAsync(
                league.Id,
                cancellationToken);

        Season activeSeason;

        // Detect new season
        if (currentSeason is null ||
            currentSeason.Key.Value != apiSeasonKey)
        {
            // Close old season
            currentSeason?.MarkAsNotCurrent();

            // Create new season
            activeSeason = new Season(
                leagueId: league.Id,
                key: SeasonKey.Create(apiSeasonKey),
                isCurrent: true);

            await _seasonRepository.AddAsync(
                activeSeason,
                cancellationToken);

            await _seasonRepository.SaveChangesAsync(cancellationToken);
        }
        else
        {
            activeSeason = currentSeason;
        }

        await _teamImportService.ImportTeamsForSeasonAsync(
            activeSeason,
            league.ExternalId,
            cancellationToken);
        
        activeSeason.MarkTeamsImported();

        DateTime? latestImportedUtc =
            await _matchImportService.ImportMatchesForSeasonAsync(
                activeSeason,
                league.ExternalId,
                cancellationToken);

        if (latestImportedUtc is not null)
        {
            activeSeason.MarkMatchesImportedUntil(latestImportedUtc.Value);
            await _seasonRepository.SaveChangesAsync(cancellationToken);
        }

        await _leagueRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
