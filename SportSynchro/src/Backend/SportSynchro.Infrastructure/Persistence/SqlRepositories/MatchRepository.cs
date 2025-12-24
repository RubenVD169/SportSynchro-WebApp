using Microsoft.EntityFrameworkCore;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence.SqlRepositories;

public sealed class MatchRepository : IMatchRepository
{
    private readonly SportSynchroDbContext _db;

    public MatchRepository(SportSynchroDbContext db)
    {
        _db = db;
    }

    public async Task<Dictionary<int, Match>> GetByExternalIdsAsync(
        int leagueId,
        IReadOnlyCollection<int> externalIds,
        CancellationToken cancellationToken = default)
    {
        if (externalIds.Count == 0)
            return [];

        return await _db.Matches
            .Where(m => m.LeagueId == leagueId && externalIds.Contains(m.ExternalId))
            .ToDictionaryAsync(
                m => m.ExternalId,
                cancellationToken);
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
}
