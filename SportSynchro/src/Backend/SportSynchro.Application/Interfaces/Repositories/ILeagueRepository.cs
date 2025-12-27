using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Interfaces.Repositories;

public interface ILeagueRepository
{
   Task<League?> GetByIdAsync(
        int leagueId,
        CancellationToken cancellationToken = default);
   
    Task<IReadOnlyList<League>> 
      GetBySportIdAsync(int sportId, CancellationToken cancellationToken);
    
    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
