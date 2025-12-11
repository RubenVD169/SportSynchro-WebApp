using SportSynchro.Domain.Exceptions;
using SportSynchro.Domain.ValueObjects;

namespace SportSynchro.Domain.Entities;

public sealed class Subscription
{
    private SubscriptionType _type;

    private Subscription() { } // EF Core only

    public Subscription(int userId, SubscriptionType type)
    {
        if (userId <= 0)
            throw new SubscriptionException("User ID must be positive.");

        _type = type ?? throw new SubscriptionException("Subscription type is required.");

        UserId = userId;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public int UserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public SubscriptionType Type => _type;

    // Business Rule: Lifetime — no expiration
    public bool IsActive() => true;

    public void UpdateType(SubscriptionType newType)
    {
        _type = newType ?? throw new SubscriptionException("Subscription type cannot be null.");
    }

}
