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

    public async Task<League?> GetByIdAsync(
        int leagueId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Leagues
            .SingleOrDefaultAsync(l => l.Id == leagueId, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }
}
