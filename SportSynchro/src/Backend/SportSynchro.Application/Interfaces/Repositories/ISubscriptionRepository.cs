using SportSynchro.Domain.Entities;

namespace SportSynchro.Application.Interfaces.Repositories;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByAspUserIdAsync(
        string aspUserId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Subscription subscription,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}