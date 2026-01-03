using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Interfaces.Repositories;

public interface ITeamRepository
{
    // ExternalId -> Team
    Task<Dictionary<int, Team>> GetByExternalIdsAsync(
        IReadOnlyCollection<int> externalIds,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Team team,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IReadOnlyList<Team> teams,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
