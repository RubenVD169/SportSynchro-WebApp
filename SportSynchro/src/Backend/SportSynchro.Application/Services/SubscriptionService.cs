using Microsoft.Extensions.Caching.Memory;
using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Subscriptions;
using SportSynchro.Domain.Entities;
using SportSynchro.Domain.ValueObjects;

namespace SportSynchro.Application.Services;

public sealed class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IMemoryCache _cache;

    public SubscriptionService(
        ISubscriptionRepository subscriptionRepository,
        IMemoryCache cache)
    {
        _subscriptionRepository = subscriptionRepository;
        _cache = cache;
    }

    public async Task ActivatePremiumAsync(ActivateSubscriptionModel model)
    {
        try
        {
            await _subscriptionRepository.AddAsync(
                new Subscription(
                    model.AspUserId,
                    SubscriptionType.Create("Premium")));

            await _subscriptionRepository.SaveChangesAsync();

            _cache.Set(
                    $"subscription:premium:user:{model.AspUserId}",
                    true,
                    TimeSpan.FromDays(1));
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while activating premium subscription.", ex);
        }
    }

    public async Task<bool> IsUserPremiumAsync(string aspUserId)
    {
        string cacheKey = $"subscription:premium:user:{aspUserId}";

        if (_cache.TryGetValue(cacheKey, out bool cached))
        {
            return cached;
        }

        Subscription? subscription =
            await _subscriptionRepository.GetByAspUserIdAsync(aspUserId);

        bool isPremium = subscription != null;

        _cache.Set(
            cacheKey,
            isPremium,
            TimeSpan.FromDays(1));

        return isPremium;
    }
}