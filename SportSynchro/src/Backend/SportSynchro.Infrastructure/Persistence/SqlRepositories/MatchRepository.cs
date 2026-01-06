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

    public Task<Match?> GetByExternalIdAsync(int externalId, CancellationToken cancellationToken = default)
    {
        return _db.Matches
            .FirstOrDefaultAsync(m => m.ExternalId == externalId, cancellationToken);
    }

    public async Task<IReadOnlyList<MatchModel>> GetScheduledMatchesByLeagueIdAsync(int leagueId, CancellationToken cancellationToken)
    {
        return await (
            from m in _db.Matches.AsNoTracking()
            join s in _db.Seasons on m.SeasonId equals s.Id
            join l in _db.Leagues on s.LeagueId equals l.Id
            join ht in _db.Teams on m.HomeTeamId equals ht.Id
            join at in _db.Teams on m.AwayTeamId equals at.Id
            where s.LeagueId == leagueId
            orderby m.StartTimeUtc
            select new MatchModel(
                m.Id,
                l.Name.Value,
                m.StartTimeUtc,
                ht.Name.Value,
                at.Name.Value,
                m.HomeScore ?? 0,
                m.AwayScore ?? 0
                ,m.Status.Value)
        ).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MatchModel>>
        GetRecentFinishedMatchesForVisibleLeaguesBySportIdAsync(
        int sportId,
        CancellationToken cancellationToken)
    {
        return await (
            from l in _db.Leagues.AsNoTracking()
            where l.IsVisible && l.SportId == sportId
            from m in
                (from m in _db.Matches
                 join s in _db.Seasons on m.SeasonId equals s.Id
                 where m.Status.Value == "Finished"
                       && s.LeagueId == l.Id
                 orderby m.StartTimeUtc descending, m.Id descending
                 select new
                 {
                     m.Id,
                     m.StartTimeUtc,
                     m.HomeScore,
                     m.AwayScore,
                     m.HomeTeamId,
                     m.AwayTeamId,
                     m.Status
                 }).Take(3)
            join ht in _db.Teams on m.HomeTeamId equals ht.Id
            join at in _db.Teams on m.AwayTeamId equals at.Id
            select new MatchModel(
                m.Id,
                l.Name.Value,
                m.StartTimeUtc,
                ht.Name.Value,
                at.Name.Value,
                m.HomeScore ?? 0,
                m.AwayScore ?? 0,
                m.Status.Value

            )
        ).ToListAsync(cancellationToken);
    }

}
