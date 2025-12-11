using SportSynchro.Domain.Exceptions;

namespace SportSynchro.Domain.ValueObjects;

public sealed class LeagueName : ValueObject
{
    public string Value { get; }

    private LeagueName() { } // EF Core

    private LeagueName(string value)
    {
        Value = value;
    }

    public static LeagueName Create(string name)
    {
        return string.IsNullOrWhiteSpace(name) ? throw new LeagueException("League name cannot be empty.") : new LeagueName(name.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
    public static implicit operator string(LeagueName name) => name.Value;
}
