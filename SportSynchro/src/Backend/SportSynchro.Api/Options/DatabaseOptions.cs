namespace SportSynchro.Api.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "ApiDatabase";
    public string ConnectionString { get; set; } = string.Empty;
}