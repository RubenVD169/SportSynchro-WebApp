using Microsoft.EntityFrameworkCore;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Domain.Entities;

namespace SportSynchro.Infrastructure.Persistence.SqlRepositories;

public sealed class SubscriptionRepository : ISubscriptionRepository
{
    private readonly SportSynchroDbContext _dbContext;

    public SubscriptionRepository(SportSynchroDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Subscription subscription, CancellationToken cancellationToken = default)
    {
        await _dbContext.Subscriptions.AddAsync(subscription, cancellationToken);
    }

    public async Task<Subscription?> GetByAspUserIdAsync(string aspUserId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.AspUserId == aspUserId, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}