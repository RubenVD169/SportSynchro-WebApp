using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Interfaces.Repositories;

public interface ISeasonRepository
{
    Task<Season?> GetByIdAsync(
        int seasonId,
        CancellationToken cancellationToken = default);
    
    Task<Season?> GetCurrentForLeagueAsync(
        int leagueId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Season season,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
