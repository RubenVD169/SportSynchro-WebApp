using SportSynchro.Application.Models.Subscriptions;

namespace SportSynchro.Application.Interfaces.Services;

public interface ISubscriptionService
{
    public Task ActivatePremiumAsync(ActivateSubscriptionModel model);
    public Task<bool> IsUserPremiumAsync(string aspUserId);
}
