using SportSynchro.Domain.Exceptions;

namespace SportSynchro.Domain.ValueObjects;

public sealed class SportName : ValueObject
{
    public string Value { get; }

    private SportName() { } // EF Core

    private SportName(string value)
    {
        Value = value;
    }

    public static SportName Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new SportException("Sport name cannot be empty.");

        return new SportName(name.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
