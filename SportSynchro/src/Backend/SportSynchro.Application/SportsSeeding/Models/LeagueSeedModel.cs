namespace SportSynchro.Application.SportsSeeding.Models;

public sealed class LeagueSeedModel
{
    public int ExternalId { get; init; }
    public string Name { get; init; } = null!;
    public IReadOnlyList<TeamSeedModel> Teams { get; init; } = [];
}
