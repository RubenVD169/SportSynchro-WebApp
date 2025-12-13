using SportSynchro.Domain.Exceptions;
using SportSynchro.Domain.ValueObjects;

namespace SportSynchro.Domain.Entities;

public sealed class Sport
{
    private SportName _name;

    private Sport() { } // EF Core only

    public Sport(int externalId, SportName name, bool isVisible)
    {
        if (externalId <= 0)
            throw new SportException("External Sport ID must be positive.");

        _name = name ?? throw new SportException("Sport name is required.");

        ExternalId = externalId;
        IsVisible = isVisible;
    }

    public int Id { get; private set; }
    public int ExternalId { get; private set; }

    public bool IsVisible { get; private set; }
    public SportName Name => _name;

    public void UpdateName(SportName name)
        => _name = name ?? throw new SportException("Sport name cannot be null.");

    public void SetVisibility(bool visible)
        => IsVisible = visible;
}
