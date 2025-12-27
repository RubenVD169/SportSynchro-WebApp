using Microsoft.EntityFrameworkCore;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence.SqlRepositories;

public sealed class SportRepository : ISportRepository
{
    private readonly SportSynchroDbContext _db;

    public SportRepository(SportSynchroDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Sport>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _db.Sports
            .AsNoTracking()
            .OrderBy(s => s.Name.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Sport>> GetAllVisibleAsync(
        CancellationToken cancellationToken = default)
    {
        return await _db.Sports
            .Where(s => s.IsVisible)
            .OrderBy(s => s.Name.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<Sport?> GetByIdAsync(
        int sportId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Sports
            .AsTracking()
            .SingleOrDefaultAsync(s => s.Id == sportId, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        int result = await _db.SaveChangesAsync(cancellationToken);
        return result;
    }
}
