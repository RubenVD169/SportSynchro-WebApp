namespace SportSynchro.Application.SportsSeeding.Models;

public sealed class SportSeedModel
{
    public int ExternalId { get; init; }
    public string Name { get; init; } = null!;
    public IReadOnlyList<LeagueSeedModel> Leagues { get; init; } = [];
}
