using Microsoft.EntityFrameworkCore;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence.SqlRepositories;

public sealed class LeagueRepository : ILeagueRepository
{
    private readonly SportSynchroDbContext _db;

    public LeagueRepository(SportSynchroDbContext db)
    {
        _db = db;
    }

    // User is only allowed to see leagues that are marked as visible
    public async Task<IReadOnlyList<League>> GetForUserBySportIdAsync(int sportId, CancellationToken cancellationToken)
    {
       return await _db.Leagues
            .Where(l => l.IsVisible && l.SportId == sportId)
            .ToListAsync(cancellationToken);
    }

    public async Task<League?> GetByIdAsync(
        int leagueId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Leagues
            .SingleOrDefaultAsync(l => l.Id == leagueId, cancellationToken);
    }

    public async Task<IReadOnlyList<League>> GetBySportIdAsync(int sportId, CancellationToken cancellationToken)
    {
        return await _db.Leagues
            .Where(l => l.SportId == sportId)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }
}
