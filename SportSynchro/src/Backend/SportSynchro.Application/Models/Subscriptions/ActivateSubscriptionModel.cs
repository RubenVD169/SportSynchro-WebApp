namespace SportSynchro.Application.Models.Subscriptions;

public sealed record ActivateSubscriptionModel(
    string AspUserId,
    string StripeSessionId);