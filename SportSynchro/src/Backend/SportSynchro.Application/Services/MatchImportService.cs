using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Domain.Entities;
using SportSynchro.Domain.Exceptions;
using SportSynchro.Domain.ValueObjects;
using SportSynchro.Infrastructure.External.TheSportsDb;
using SportSynchro.Infrastructure.External.TheSportsDb.Models.Match;
using SportSynchro.Infrastructure.Persistence;

namespace SportSynchro.Application.Services;

public sealed class MatchImportService : IMatchImportService
{
    private readonly SportSynchroDbContext _db;
    private readonly ITheSportsDbRepository _theSportsDb;

    public MatchImportService(
        SportSynchroDbContext db,
        ITheSportsDbRepository theSportsDb)
    {
        _db = db;
        _theSportsDb = theSportsDb;
    }

    public async Task<DateTime?> ImportMatchesForLeagueAsync(
        League league,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(league);

        if (!league.TeamsImported)
            throw new MatchException("Cannot import matches before teams are imported.");

        // Guard: TheSportsDB schedules only supported for Soccer 
        string sportName = await _db.Sports
            .Where(s => s.Id == league.SportId)
            .Select(s => s.Name.Value)
            .SingleAsync(cancellationToken);

        if (!string.Equals(sportName, "Soccer", StringComparison.OrdinalIgnoreCase))
        {
            throw new MatchException(
                $"Match import is only supported for Soccer. League {league.Id} belongs to sport '{sportName}'.");
        }

        // Get teams in this league
        var teams = await _db.Teams
            .Where(t => t.LeagueId == league.Id)
            .Select(t => new { t.Id, t.ExternalId })
            .ToListAsync(cancellationToken);

        if (teams.Count == 0)
            throw new MatchException($"No teams found for league {league.Id}. Cannot import matches.");

        Dictionary<int, int> teamLookup = teams.ToDictionary(t => t.ExternalId, t => t.Id);

        // Get external matches for these teams 
        Dictionary<int, TheSportsDbMatchDto> byExternalMatchId = [];

        foreach (var team in teams)
        {
            IReadOnlyList<TheSportsDbMatchDto> teamMatches = await _theSportsDb.GetMatchesByTeamAsync(team.ExternalId, cancellationToken);

            foreach (TheSportsDbMatchDto dto in teamMatches)
            {
                if (dto.IdEvent <= 0) continue;

                // Dedup op external match id
                byExternalMatchId.TryAdd(dto.IdEvent, dto);
            }
        }

        if (byExternalMatchId.Count == 0)
            return null;

        // 3) Bulk existing matches from DB
        IEnumerable<int> externalIds = [.. byExternalMatchId.Keys];

        List<Match> existing = await _db.Matches
            .Where(m => m.LeagueId == league.Id && externalIds.Contains(m.ExternalId))
            .ToListAsync(cancellationToken);

        Dictionary<int, Match> existingByExternalId = existing.ToDictionary(m => m.ExternalId);

        // 4) Upsert
        DateTime? maxStartUtc = null;

        foreach (TheSportsDbMatchDto dto in byExternalMatchId.Values)
        {
            // mapping teams (external -> internal)
            // Skip non-league matches (e.g. cups, friendlies, internationals)
            if (!teamLookup.TryGetValue(dto.IdHomeTeam, out int homeTeamId) ||
                !teamLookup.TryGetValue(dto.IdAwayTeam, out int awayTeamId))
            {
                continue;
            }

            DateTime startUtc = ParseStartUtc(dto.StrTimestamp, dto.IdEvent);

            maxStartUtc = maxStartUtc is null || startUtc > maxStartUtc.Value
                ? startUtc
                : maxStartUtc;

            MatchStatus status = MapStatus(dto.StrStatus);
            int? homeScore = dto.IntHomeScore;
            int? awayScore = dto.IntAwayScore;
            int? round = dto.IntRound;

            if (!existingByExternalId.TryGetValue(dto.IdEvent, out Match? match))
            {
                match = new Match(
                    externalId: dto.IdEvent,
                    leagueId: league.Id,
                    homeTeamId: homeTeamId,
                    awayTeamId: awayTeamId,
                    startTimeUtc: startUtc,
                    status: status,
                    roundNumber: round,
                    homeScore: homeScore,
                    awayScore: awayScore
                );

                _db.Matches.Add(match);
                continue;
            }

            // Update existing match
            match.UpdateScore(homeScore, awayScore);
            match.UpdateStatus(status);
        }

        return maxStartUtc;
    }

    private static DateTime ParseStartUtc(string strTimestamp, int externalMatchId)
    {
        if (string.IsNullOrWhiteSpace(strTimestamp))
            throw new MatchException($"Match {externalMatchId} is missing strTimestamp.");

        // Voorbeelden zijn ISO strings; we behandelen ze als UTC/Universal.
        return !DateTime.TryParse(
            strTimestamp,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out DateTime parsed) 
            ? throw new MatchException($"Match {externalMatchId} has invalid strTimestamp '{strTimestamp}'.") 
            : DateTime.SpecifyKind(parsed, DateTimeKind.Utc);
    }

    private static MatchStatus MapStatus(string externalStatus)
    {
        if (string.IsNullOrWhiteSpace(externalStatus))
            throw new MatchException("External match status is missing.");

        string status = externalStatus.Trim().ToUpperInvariant();

        string normalized = status switch
        {
            // Not started
            "TBD" or "NS" or "NOT STARTED" 
                => "Not Started",

            // In progress
            "1H" or "HT" or "2H" or "ET" or "P" or "BT"
                => "In Progress",

            // Finished
            "FT" or "AET" or "PEN" or "AWD" or "WO" or "MATCH FINISHED" or "FINISHED" 
                => "Finished",

            // Postponed / interrupted / cancelled
            "PST" or "CANC" or "ABD" or "SUSP" or "INT" or "POSTPONED"
                => "Postponed",

            // Fallback (defensive)
            _ => throw new MatchException($"Unknown external match status: {externalStatus}")
        };

        return MatchStatus.Create(normalized);
    }

}
