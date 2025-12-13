namespace SportSynchro.Domain.ValueObjects;

// Represents an immutable domain Value Object.
// Equality is based on the values of its components.
public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;
        return Equals((ValueObject)obj);
    }

    public bool Equals(ValueObject? other)
    {
        if (other is null || other.GetType() != GetType())
            return false;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        foreach (object? component in GetEqualityComponents())
            hash.Add(component);
        return hash.ToHashCode();
    }
}