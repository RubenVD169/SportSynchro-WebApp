namespace SportSynchro.IdentityServer.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "IdentityServerDatabase";
    public string ConnectionString { get; set; } = string.Empty;
}
