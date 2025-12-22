namespace SportSynchro.Application.SportsSeeding.Models;

public sealed class TeamSeedModel
{
    public int ExternalId { get; init; }
    public string Name { get; init; } = null!;
    public string Country { get; init; } = null!;
}
