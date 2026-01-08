namespace SportSynchro.IdentityServer.Options;

public sealed class IdentityServerClientsOptions
{
    public string WebClientRedirectUri { get; init; } 
    public string WebClientPostLogoutRedirectUri { get; init; }
    public string WebClientCorsOrigin { get; init; }

    public string LiveScoreClientSecret { get; init; }
}
