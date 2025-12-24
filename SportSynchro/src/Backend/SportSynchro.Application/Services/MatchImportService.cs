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
    private readonly ITeamRepository _teamRepository;
    private readonly IMatchRepository _matchRepository;

    public MatchImportService(
        ITheSportsDbRepository sportsDb,
        ITeamRepository teamRepository,
        IMatchRepository matchRepository)
    {
        _sportsDb = sportsDb;
        _teamRepository = teamRepository;
        _matchRepository = matchRepository;
    }

    public async Task<DateTime?> ImportMatchesForLeagueAsync(
        League league,
        CancellationToken cancellationToken = default)
    {
        // Safety: matches require teams
        if (!league.TeamsImported)
            return null;

        // Load all teams for league (externalId -> teamId)
        Dictionary<int, int> teamLookup =
            await _teamRepository.GetTeamLookupForLeagueAsync(
                league.Id,
                cancellationToken);

        if (teamLookup.Count == 0)
            return null;

        Dictionary<int, TheSportsDbMatchDto> uniqueMatches = [];

        // Fetch matches per team
        foreach (int teamExternalId in teamLookup.Keys)
        {
            IReadOnlyList<TheSportsDbMatchDto> teamMatches =
                await _sportsDb.GetMatchesByTeamAsync(
                    teamExternalId,
                    cancellationToken);

            foreach (TheSportsDbMatchDto dto in teamMatches)
            {
                int matchExternalId = dto.IdEvent;

                uniqueMatches.TryAdd(matchExternalId, dto);
            }
        }

        if (uniqueMatches.Count == 0)
            return null;

        // Detect already imported matches
        Dictionary<int, Match> existingMatches =
            await _matchRepository.GetByExternalIdsAsync(
                league.Id,
                uniqueMatches.Keys,
                cancellationToken);

        DateTime? latestImportedUtc = null;

        foreach ((int externalId, TheSportsDbMatchDto dto) in uniqueMatches)
        {
            if (existingMatches.ContainsKey(externalId))
                continue;

            if (!TryMapMatch(
                    dto,
                    league,
                    teamLookup,
                    out Match? match,
                    out DateTime? startUtc))
            {
                continue;
            }

            await _matchRepository.AddAsync(match!, cancellationToken);

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
        League league,
        Dictionary<int, int> teamLookup,
        out Match? match,
        out DateTime? startUtc)
    {
        match = null;
        startUtc = null;

        int homeExternalId = dto.IdHomeTeam;
        int awayExternalId = dto.IdAwayTeam;

        if (homeExternalId <= 0 || awayExternalId <= 0)
            return false;

        if (!teamLookup.TryGetValue(homeExternalId, out int homeTeamId))
            return false;

        if (!teamLookup.TryGetValue(awayExternalId, out int awayTeamId))
            return false;

        if (!TryParseStartUtc(dto, out DateTime parsedStartUtc))
            return false;

        MatchStatus status = MapStatus(dto.StrStatus);

        match = new Match(
            externalId: dto.IdEvent,
            leagueId: league.Id,
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

        if (!DateTime.TryParse(
                dto.StrTimestamp,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out DateTime parsed))
        {
            return false;
        }

        startUtc = parsed;
        return true;
    }


    private static MatchStatus MapStatus(string? status)
    {
        return status switch
        {
            "NS" =>
                MatchStatus.Create("Not Started"),
            "FT" or "AET" or "PEN" =>
                MatchStatus.Create("Finished"),
            "1H" or "HT" or "2H" or "ET" or "BT" or "P" =>
                MatchStatus.Create("In Progress"),
            "PST" or "CANC" or "ABD" or "AWD" or "WO" =>
                MatchStatus.Create("Postponed"),
            _ => 
                MatchStatus.Create("Not Started")
        };
    }
}
