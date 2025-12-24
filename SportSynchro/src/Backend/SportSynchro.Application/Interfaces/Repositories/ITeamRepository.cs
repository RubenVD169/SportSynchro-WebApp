using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Interfaces.Repositories;

public interface ITeamRepository
{
    Task<HashSet<int>> GetExistingExternalIdsForLeagueAsync(
            int leagueId,
            CancellationToken cancellationToken = default);

    Task<Dictionary<int, int>> GetTeamLookupForLeagueAsync(
    int leagueId,
    CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IReadOnlyList<Team> teams,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
