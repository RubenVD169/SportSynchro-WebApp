using Microsoft.EntityFrameworkCore;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Models.Matches;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence.SqlRepositories;

public sealed class MatchRepository : IMatchRepository
{
    private readonly SportSynchroDbContext _db;

    public MatchRepository(SportSynchroDbContext db)
    {
        _db = db;
    }

    public Task<List<Match>> GetByExternalIdsAsync(
    IReadOnlyCollection<int> externalIds,
    CancellationToken ct)
    {
        return _db.Matches
            .Where(m => externalIds.Contains(m.ExternalId))
            .ToListAsync(ct);
    }

    public async Task<HashSet<int>> GetExistingExternalIdsForSeasonAsync(
    int seasonId,
    IReadOnlyCollection<int> externalIds,
    CancellationToken cancellationToken = default)
    {
        return await _db.Matches
            .Where(m => m.SeasonId == seasonId && externalIds.Contains(m.ExternalId))
            .Select(m => m.ExternalId)
            .ToHashSetAsync(cancellationToken);
    }

    public async Task AddAsync(
        Match match,
        CancellationToken cancellationToken = default)
    {
        await _db.Matches.AddAsync(match, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MatchModel>> GetRecentFinishedMatchesByLeagueIdAsync(
    int leagueId,
    CancellationToken cancellationToken)
    {
        return await _db.Matches
            .AsNoTracking()
            .Where(m => m.Status.Value == "Finished")
            .Join(
                _db.Seasons.AsNoTracking(),
                match => match.SeasonId,
                season => season.Id,
                (match, season) => new { match, season })
            .Where(x => x.season.LeagueId == leagueId)
            .Join(
                _db.Leagues.AsNoTracking(),
                ms => ms.season.LeagueId,
                league => league.Id,
                (ms, league) => new { ms.match, league })
            .Join(
                _db.Teams.AsNoTracking(),
                ml => ml.match.HomeTeamId,
                homeTeam => homeTeam.Id,
                (ml, homeTeam) => new { ml.match, ml.league, homeTeam })
            .Join(
                _db.Teams.AsNoTracking(),
                mlh => mlh.match.AwayTeamId,
                awayTeam => awayTeam.Id,
                (mlh, awayTeam) => new { mlh.match, mlh.league, mlh.homeTeam, awayTeam })
            .OrderByDescending(x => x.match.StartTimeUtc)
            .Take(10)
            .Select(x => new MatchModel(
                x.match.Id,
                x.league.Name.Value,
                x.match.StartTimeUtc,
                x.homeTeam.Name.Value,
                x.awayTeam.Name.Value,
                x.match.HomeScore ?? 0,
                x.match.AwayScore ?? 0))
            .ToListAsync(cancellationToken);
    }

    public Task<Match?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default)
    {
        return _db.Matches
            .FirstOrDefaultAsync(m => m.ExternalId == externalId, cancellationToken);
    }
}
