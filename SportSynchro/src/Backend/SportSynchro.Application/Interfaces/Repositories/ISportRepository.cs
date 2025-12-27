using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Interfaces.Repositories;

public interface ISportRepository
{
    Task<IReadOnlyList<Sport>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Sport?> GetByIdAsync(
        int sportId,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
