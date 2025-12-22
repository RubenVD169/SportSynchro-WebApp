namespace SportSynchro.Infrastructure.Options;

public sealed class TheSportsDbOptions
{
    public required string BaseUrl { get; init; }
    public required string ApiKey { get; init; }
}