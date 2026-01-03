namespace SportSynchro.Api.Options.ExternalOptions;

public sealed class TheSportsDbOptions
{
    public const string SectionName = "TheSportsDb";
    public required string BaseUrl { get; init; }
    public required string ApiKey { get; init; }
}