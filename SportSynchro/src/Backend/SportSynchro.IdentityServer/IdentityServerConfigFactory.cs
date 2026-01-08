using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;
using Microsoft.Extensions.Options;
using SportSynchro.IdentityServer.Options;

namespace SportSynchro.IdentityServer;

public sealed class IdentityServerConfigFactory
{
    private readonly IdentityServerClientsOptions _options;

    public IdentityServerConfigFactory(
        IOptions<IdentityServerClientsOptions> options)
    {
        _options = options.Value;
    }

    public IEnumerable<IdentityResource> GetIdentityResources() =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource(
                "roles",
                "User roles",
                new[] { JwtClaimTypes.Role })
        };

    public IEnumerable<ApiScope> GetApiScopes() =>
        new ApiScope[]
        {
            new("sportsynchro.api.read", "Read access to SportSynchro API", new[] { JwtClaimTypes.Role }),
            new("sportsynchro.api.write", "Write access to SportSynchro API", new[] { JwtClaimTypes.Role }),
            new("sportsynchro.livescore.read", "Read access to LiveScore API") { Required = true }
        };

    public IEnumerable<ApiResource> GetApiResources() =>
        new ApiResource[]
        {
            new("sportsynchro.api", "SportSynchro API")
            {
                Scopes = { "sportsynchro.api.read", "sportsynchro.api.write" },
                UserClaims = { JwtClaimTypes.Role }
            },
            new("sportsynchro.livescore.api", "SportSynchro LiveScore API")
            {
                Scopes = { "sportsynchro.livescore.read" }
            }
        };

    public IEnumerable<Client> GetClients() =>
        new Client[]
        {
            new()
            {
                ClientId = "m2m.postman",
                ClientName = "Postman Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("postman-dev-secret".Sha256())
                },
                AllowedScopes =
                {
                    "sportsynchro.api.read",
                    "sportsynchro.api.write",
                    "sportsynchro.livescore.read"
                }
            },

            new()
            {
                ClientId = "webapp-client",
                RequireClientSecret = false,
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireConsent = false,

                AlwaysIncludeUserClaimsInIdToken = true,

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "roles",
                    "sportsynchro.api.read",
                    "sportsynchro.api.write"
                },

                RedirectUris = { _options.WebClientRedirectUri },
                PostLogoutRedirectUris = { _options.WebClientPostLogoutRedirectUri },
                AllowedCorsOrigins = { _options.WebClientCorsOrigin },

                AllowAccessTokensViaBrowser = true
            },

            new()
            {
                ClientId = "sportsynchro.main.livescore",
                ClientName = "SportSynchro Main API → LiveScore API",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret(_options.LiveScoreClientSecret.Sha256())
                },
                AllowedScopes = { "sportsynchro.livescore.read" }
            }
        };
}
