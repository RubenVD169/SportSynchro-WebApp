namespace SportSynchro.Api.Options;

public sealed class LiveScoreApiOptions
{
    public const string SectionName = "LiveScoreApi";
    public string BaseUrl { get; set; } = null!;
}
