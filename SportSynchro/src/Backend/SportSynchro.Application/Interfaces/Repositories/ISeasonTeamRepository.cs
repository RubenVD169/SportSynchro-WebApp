namespace SportSynchro.Application.Interfaces.Repositories;

public interface ISeasonTeamRepository
{
    // Returns a lookup: TeamExternalId -> TeamId
    Task<Dictionary<int, int>> GetTeamLookupForSeasonAsync(
        int seasonId,
        CancellationToken cancellationToken = default);

    Task<HashSet<int>> GetTeamIdsForSeasonAsync(
    int seasonId,
    CancellationToken cancellationToken = default);

    Task AddAsync(
        SeasonTeam seasonTeam,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
