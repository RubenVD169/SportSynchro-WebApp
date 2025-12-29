using SportSynchro.Application.Models.Matches;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Interfaces.Repositories;

public interface IMatchRepository
{
   Task<Dictionary<int, Match>> GetByExternalIdsAsync(
        int leagueId,
        IReadOnlyCollection<int> externalIds,
        CancellationToken cancellationToken = default);

    Task<HashSet<int>> GetExistingExternalIdsForSeasonAsync(
     int seasonId,
     IReadOnlyCollection<int> externalIds,
     CancellationToken cancellationToken = default);

    Task AddAsync(
        Match match,
        CancellationToken cancellationToken = default);

   Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MatchModel>> GetRecentFinishedMatchesByLeagueIdAsync(
      int leagueId, CancellationToken cancellationToken);
}
