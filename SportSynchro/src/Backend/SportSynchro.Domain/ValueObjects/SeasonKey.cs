using SportSynchro.Domain.Exceptions;

namespace SportSynchro.Domain.ValueObjects;

public sealed class SeasonKey : ValueObject
{
    public string Value { get; }

    private SeasonKey() { } // EF Core

    private SeasonKey(string value)
    {
        Value = value;
    }

    public static SeasonKey Create(string key)
    {
        return string.IsNullOrWhiteSpace(key) 
            ? throw new SeasonException("Season key cannot be empty.") 
            : new SeasonKey(key.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
    public static implicit operator string(SeasonKey key) => key.Value;
}
