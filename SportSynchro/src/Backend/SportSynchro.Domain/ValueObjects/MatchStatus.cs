using SportSynchro.Domain.Exceptions;

namespace SportSynchro.Domain.ValueObjects;

public sealed class MatchStatus : ValueObject
{
    public string Value { get; }

    private MatchStatus() { } // EF Core only

    private MatchStatus(string value)
    {
        Value = value;
    }

    public static MatchStatus Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new MatchException("Match status cannot be empty.");

        value = value.Trim();

        return !AllowedStatuses.Contains(value) ? throw new MatchException($"Invalid match status: {value}") : new MatchStatus(value);
    }

    public static readonly HashSet<string> AllowedStatuses =
    [
        "Not Started",
        "In Progress",
        "Finished",
        "Postponed"
    ];

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
