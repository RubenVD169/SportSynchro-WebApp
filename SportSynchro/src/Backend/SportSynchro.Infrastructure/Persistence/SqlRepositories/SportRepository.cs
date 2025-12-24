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

    public async Task<Sport?> GetByIdAsync(
        int sportId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Sports
            .SingleOrDefaultAsync(s => s.Id == sportId, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }
}
