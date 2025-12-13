using SportSynchro.Domain.Exceptions;

namespace SportSynchro.Domain.ValueObjects;

public sealed class SubscriptionType : ValueObject
{
    public string Value { get; }

    private SubscriptionType() { } // EF Core

    private SubscriptionType(string value)
    {
        Value = value;
    }

    public static readonly SubscriptionType Free = new("Free");
    public static readonly SubscriptionType Premium = new("Premium");

    public static SubscriptionType Create(string value)
    {
        value = value.Trim();

        return !AllowedTypes.Contains(value)
            ? throw new SubscriptionException($"Unsupported subscription type: {value}")
            : new SubscriptionType(value);
    }

    private static readonly HashSet<string> AllowedTypes =
    [
        "Free",
        "Premium"
    ];

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
