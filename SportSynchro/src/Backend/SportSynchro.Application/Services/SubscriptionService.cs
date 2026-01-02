using SportSynchro.Application.Interfaces.Repositories;
using SportSynchro.Application.Interfaces.Services;
using SportSynchro.Application.Models.Subscriptions;
using SportSynchro.Domain.Entities;
using SportSynchro.Domain.ValueObjects;

namespace SportSynchro.Application.Services;

public sealed class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public SubscriptionService(
        ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task ActivatePremiumAsync(ActivateSubscriptionModel model)
    {
        try{
            await _subscriptionRepository.AddAsync(
                new Subscription(
                    model.AspUserId,
                    SubscriptionType.Create("Premium")));
        
            await _subscriptionRepository.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            // Log the exception or handle it as needed
            throw new ApplicationException("An error occurred while activating premium subscription.", ex);
        }
    }

    public async Task<bool> IsUserPremiumAsync(string aspUserId)
    {
        Subscription? subscription = await _subscriptionRepository.GetByAspUserIdAsync(aspUserId);
        return subscription != null;
    }
}