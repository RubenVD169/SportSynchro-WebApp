namespace SportSynchro.Api.Options;

public sealed class LiveScoreAuthOptions
{
    public const string SectionName = "LiveScoreAuth";

    public string Authority { get; init; }
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
    public string Scope { get; init; }
    public string Audience { get; init; }
}
