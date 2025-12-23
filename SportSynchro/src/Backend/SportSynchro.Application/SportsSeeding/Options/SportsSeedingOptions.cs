namespace SportSynchro.Application.SportsSeeding.Options;

public sealed class SportsSeedingOptions
{
    public bool Enabled { get; init; } = false;

    public IReadOnlyList<string> Sports { get; init; } = [];
}
