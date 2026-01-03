namespace SportSynchro.LiveScore.Api.Options;

public sealed class SportSynchroApiAuthOptions
{
    public const string SectionName = "SportSynchroApiAuth";

    public string Authority { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string Scope { get; set; } = null!;
}