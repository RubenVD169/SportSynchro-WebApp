namespace SportSynchro.LiveScore.Api.Options;

public sealed class IdentityServerOptions
{
    public const string SectionName = "IdentityServer";

    public required string Authority { get; init; }
    public required string Audience { get; init; }
}
