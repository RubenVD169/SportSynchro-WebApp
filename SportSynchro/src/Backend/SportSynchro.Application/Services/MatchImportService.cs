using System.Globalization;
using SportSynchro.Application.Interfaces.External;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Domain.Entities;
using SportSynchro.Domain.ValueObjects;
using SportSynchro.External.TheSportsDb.Contracts.Models.Matches;

namespace SportSynchro.Application.Services;

public sealed class MatchImportService : IMatchImportService
{
    private readonly ITheSportsDbRepository _sportsDb;
    private readonly ISeasonTeamRepository _seasonTeamRepository;
    private readonly IMatchRepository _matchRepository;

    public MatchImportService(
        ITheSportsDbRepository sportsDb,
        ISeasonTeamRepository seasonTeamRepository,
        IMatchRepository matchRepository)
    {
        _sportsDb = sportsDb;
        _seasonTeamRepository = seasonTeamRepository;
        _matchRepository = matchRepository;
    }

    public async Task<DateTime?> ImportMatchesForSeasonAsync(
        Season season,
        int leagueExternalId,
        CancellationToken cancellationToken = default)
    {
        // Get all teams participating in this season
        // ExternalTeamId -> InternalTeamId
        Dictionary<int, int> teamLookup =
            await _seasonTeamRepository.GetTeamLookupForSeasonAsync(
                season.Id,
                cancellationToken);

        if (teamLookup.Count == 0)
            return null;

        // Fetch all matches for this league + season in a single API call
        IReadOnlyList<TheSportsDbMatchDto> apiMatches =
            await _sportsDb.GetMatchesByLeagueAndSeasonAsync(
                leagueExternalId,
                season.Key.Value,
                cancellationToken);

        if (apiMatches.Count == 0)
            return null;

        // Detect which matches are already imported (idempotency guard)
        HashSet<int> existingMatchExternalIds =
            await _matchRepository.GetExistingExternalIdsForSeasonAsync(
                season.Id,
                apiMatches.Select(m => m.IdEvent).ToHashSet(),
                cancellationToken);

        DateTime? latestImportedUtc = null;

        foreach (TheSportsDbMatchDto dto in apiMatches)
        {
            // Skip already imported matches
            if (existingMatchExternalIds.Contains(dto.IdEvent))
                continue;

            if (!TryMapMatch(
                    dto,
                    season,
                    teamLookup,
                    out Match? match,
                    out DateTime? startUtc))
            {
                continue;
            }

            await _matchRepository.AddAsync(match!, cancellationToken);

            // Track the latest imported match start time
            if (startUtc is not null &&
                (latestImportedUtc is null || startUtc > latestImportedUtc))
            {
                latestImportedUtc = startUtc;
            }
        }

        await _matchRepository.SaveChangesAsync(cancellationToken);
        return latestImportedUtc;
    }

    private static bool TryMapMatch(
        TheSportsDbMatchDto dto,
        Season season,
        Dictionary<int, int> teamLookup,
        out Match? match,
        out DateTime? startUtc)
    {
        match = null;
        startUtc = null;

        // Resolve home team
        if (!teamLookup.TryGetValue(dto.IdHomeTeam, out int homeTeamId))
            return false;

        // Resolve away team
        if (!teamLookup.TryGetValue(dto.IdAwayTeam, out int awayTeamId))
            return false;

        // Parse start time
        if (!TryParseStartUtc(dto, out DateTime parsedStartUtc))
            return false;

        MatchStatus status =
            MapStatus(dto.StrStatus.ToUpperInvariant());

        match = new Match(
            externalId: dto.IdEvent,
            seasonId: season.Id,
            homeTeamId: homeTeamId,
            awayTeamId: awayTeamId,
            startTimeUtc: parsedStartUtc,
            status: status);

        startUtc = parsedStartUtc;
        return true;
    }

    private static bool TryParseStartUtc(
        TheSportsDbMatchDto dto,
        out DateTime startUtc)
    {
        startUtc = default;

        if (string.IsNullOrWhiteSpace(dto.StrTimestamp))
            return false;

        return DateTime.TryParse(
            dto.StrTimestamp,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out startUtc);
    }

    private static MatchStatus MapStatus(string? status)
    {
        return status switch
        {
            "NS" =>
                MatchStatus.Create("Not Started"),

            "FT" or "AET" or "PEN" or "MATCH FINISHED" =>
                MatchStatus.Create("Finished"),

            "1H" or "HT" or "2H" or "ET" or "BT" or "P" or "IN PROGRESS" =>
                MatchStatus.Create("In Progress"),

            "PST" or "CANC" or "ABD" or "AWD" or "WO" or "POSTPONED" =>
                MatchStatus.Create("Postponed"),

            _ =>
                MatchStatus.Create("Not Started")
        };
    }
}
