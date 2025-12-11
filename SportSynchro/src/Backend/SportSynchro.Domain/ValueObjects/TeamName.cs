using SportSynchro.Domain.Exceptions;

namespace SportSynchro.Domain.ValueObjects;

public sealed class TeamName : ValueObject
{
    public string Value { get; }

    private TeamName() { } // EF Core only

    private TeamName(string value)
    {
        Value = value;
    }

    public static TeamName Create(string name)
    {
        return string.IsNullOrWhiteSpace(name) ? throw new TeamException("Team name cannot be empty.") : new TeamName(name.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
