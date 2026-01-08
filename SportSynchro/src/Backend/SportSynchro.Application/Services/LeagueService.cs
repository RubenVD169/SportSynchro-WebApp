using Microsoft.Extensions.Caching.Memory;
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
    private readonly ISeasonResolver _seasonResolver;
    private readonly IMemoryCache _cache;

    private static string UserLeaguesCacheKey(int sportId)
        => $"visible-leagues-{sportId}";

    private static string AdminLeaguesCacheKey(int sportId)
        => $"admin-leagues-{sportId}";

    public LeagueService(
        ILeagueRepository leagueRepository,
        ISeasonRepository seasonRepository,
        ITeamImportService teamImportService,
        IMatchImportService matchImportService,
        ITheSportsDbRepository sportsDbRepository,
        ISeasonResolver seasonResolver,
        IMemoryCache memoryCache)
    {
        _leagueRepository = leagueRepository;
        _seasonRepository = seasonRepository;
        _teamImportService = teamImportService;
        _matchImportService = matchImportService;
        _sportsDbRepository = sportsDbRepository;
        _seasonResolver = seasonResolver;
        _cache = memoryCache;
    }

    public async Task<IReadOnlyList<LeagueUserModel>>
        GetAllForUserBySportIdAsync(
            int sportId,
            CancellationToken cancellationToken)
    {
        string cacheKey = UserLeaguesCacheKey(sportId);

        if (_cache.TryGetValue(
                cacheKey,
                out IReadOnlyList<LeagueUserModel>? cached)
            && cached is not null)
        {
            return cached;
        }

        IReadOnlyList<League> leagues =
            await _leagueRepository.GetForUserBySportIdAsync(
                sportId,
                cancellationToken);

        IReadOnlyList<LeagueUserModel> result =
        [
            .. leagues.Select(l => new LeagueUserModel(
                l.Id,
                l.Name.Value))
        ];

        _cache.Set(cacheKey, result);

        return result;
    }

    public async Task<IReadOnlyList<LeagueAdminModel>>
        GetLeaguesForAdminBySportIdAsync(
            int sportId,
            CancellationToken cancellationToken)
    {
        string cacheKey = AdminLeaguesCacheKey(sportId);

        if (_cache.TryGetValue(
                cacheKey,
                out IReadOnlyList<LeagueAdminModel>? cached)
            && cached is not null)
        {
            return cached;
        }

        IReadOnlyList<League> leagues =
            await _leagueRepository.GetBySportIdAsync(
                sportId,
                cancellationToken);

        IReadOnlyList<LeagueAdminModel> result =
        [
            .. leagues.Select(l => new LeagueAdminModel(
                l.Id,
                l.Name.Value,
                l.IsVisible))
        ];

        _cache.Set(cacheKey, result);

        return result;
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
            InvalidateLeagueCaches(league.SportId);
            return true;
        }

        IReadOnlyList<string> seasons =
            await _sportsDbRepository.GetSeasonsByLeagueAsync(
                league.ExternalId,
                cancellationToken);

        string apiSeasonKey =
            seasons.Count == 0
                ? "default"
                : seasons[^1]; //last season

        Season? currentSeason =
            await _seasonRepository.GetCurrentForLeagueAsync(
                league.Id,
                cancellationToken);

        Season activeSeason;
        bool seasonSwitched = false;
        // Detect new season        
        if (currentSeason is null ||
            currentSeason.Key.Value != apiSeasonKey)
        {
            currentSeason?.MarkAsNotCurrent();
            //close old season
            activeSeason = new Season(
                leagueId: league.Id,
                key: SeasonKey.Create(apiSeasonKey),
                isCurrent: true);

            await _seasonRepository.AddAsync(
                activeSeason,
                cancellationToken);

            await _seasonRepository.SaveChangesAsync(cancellationToken);

            seasonSwitched = true;
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

        if (seasonSwitched)
        {
            _seasonResolver.Invalidate(league.Id);
        }

        InvalidateLeagueCaches(league.SportId);
        return true;
    }

    private void InvalidateLeagueCaches(int sportId)
    {
        _cache.Remove(UserLeaguesCacheKey(sportId));
        _cache.Remove(AdminLeaguesCacheKey(sportId));
    }
}
