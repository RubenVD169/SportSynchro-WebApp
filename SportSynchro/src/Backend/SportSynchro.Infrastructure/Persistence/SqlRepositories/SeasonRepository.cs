using Microsoft.EntityFrameworkCore;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence.SqlRepositories;

public sealed class SeasonRepository : ISeasonRepository
{
    private readonly SportSynchroDbContext _db;

    public SeasonRepository(SportSynchroDbContext db)
    {
        _db = db;
    }

    public async Task<Season?> GetByIdAsync(
        int seasonId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Seasons
            .FirstOrDefaultAsync(s => s.Id == seasonId, cancellationToken);
    }

    public async Task AddAsync(
        Season season,
        CancellationToken cancellationToken = default)
    {
        await _db.Seasons.AddAsync(season, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Season?> GetCurrentForLeagueAsync(
    int leagueId,
    CancellationToken cancellationToken = default)
    {
        return await _db.Seasons
            .FirstOrDefaultAsync(
                s => s.LeagueId == leagueId && s.IsCurrent,
                cancellationToken);
    }

}
