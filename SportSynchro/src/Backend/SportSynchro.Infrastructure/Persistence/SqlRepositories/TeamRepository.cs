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

    public async Task<HashSet<int>> GetExistingExternalIdsForLeagueAsync(
        int leagueId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Teams
            // .Where(t => t.LeagueId == leagueId)
            .Select(t => t.ExternalId)
            .ToHashSetAsync(cancellationToken);
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
    
    public async Task<Dictionary<int, int>> GetTeamLookupForLeagueAsync(
    int leagueId,
    CancellationToken cancellationToken = default)
    {
        return await _db.Teams
            // .Where(t => t.LeagueId == leagueId) //TODO
            .Select(t => new { t.ExternalId, t.Id })
            .ToDictionaryAsync(
                x => x.ExternalId,
                x => x.Id,
                cancellationToken);
    }
}
