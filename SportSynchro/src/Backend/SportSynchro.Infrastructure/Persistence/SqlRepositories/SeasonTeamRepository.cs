using Microsoft.EntityFrameworkCore;
using SportSynchro.Application.Interfaces.Repositories;

namespace SportSynchro.Infrastructure.Persistence.SqlRepositories;

public sealed class SeasonTeamRepository : ISeasonTeamRepository
{
    private readonly SportSynchroDbContext _db;

    public SeasonTeamRepository(SportSynchroDbContext db)
    {
        _db = db;
    }

    public async Task<Dictionary<int, int>> GetTeamLookupForSeasonAsync(
        int seasonId,
        CancellationToken cancellationToken = default)
    {
        return await _db.SeasonTeams
            .Where(st => st.SeasonId == seasonId)
            .Join(
                _db.Teams,
                st => st.TeamId,
                t => t.Id,
                (st, t) => new { t.ExternalId, t.Id })
            .ToDictionaryAsync(
                x => x.ExternalId,
                x => x.Id,
                cancellationToken);
    }

    public async Task AddAsync(
        SeasonTeam seasonTeam,
        CancellationToken cancellationToken = default)
    {
        await _db.SeasonTeams.AddAsync(seasonTeam, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<HashSet<int>> GetTeamIdsForSeasonAsync(
    int seasonId,
    CancellationToken cancellationToken = default)
    {
        return await _db.SeasonTeams
            .Where(st => st.SeasonId == seasonId)
            .Select(st => st.TeamId)
            .ToHashSetAsync(cancellationToken);
    }
}
