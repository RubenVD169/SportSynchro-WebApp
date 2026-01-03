using Microsoft.EntityFrameworkCore;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence.SqlRepositories;

public sealed class TeamRepository : ITeamRepository
{
    private readonly SportSynchroDbContext _db;

    public TeamRepository(SportSynchroDbContext db)
    {
        _db = db;
    }

    public async Task<Dictionary<int, Team>> GetByExternalIdsAsync(
        IReadOnlyCollection<int> externalIds,
        CancellationToken cancellationToken = default)
    {
        if (externalIds.Count == 0)
            return [];

        return await _db.Teams
            .Where(t => externalIds.Contains(t.ExternalId))
            .ToDictionaryAsync(
                t => t.ExternalId,
                cancellationToken);
    }

    public async Task AddAsync(
        Team team,
        CancellationToken cancellationToken = default)
    {
        await _db.Teams.AddAsync(team, cancellationToken);
    }

    public async Task AddRangeAsync(
        IReadOnlyList<Team> teams,
        CancellationToken cancellationToken = default)
    {
        if (teams.Count == 0)
            return;

        await _db.Teams.AddRangeAsync(teams, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }
}
