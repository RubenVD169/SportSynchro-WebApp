using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace SportSynchro.IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource(
                name: "roles",
                displayName: "User roles",
                userClaims: new[] { JwtClaimTypes.Role }
            )
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("sportsynchro.api.read", "Read access to SportSynchro API"),
            new ApiScope("sportsynchro.api.write", "Write access to SportSynchro API"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // m2m client credentials flow postman
            new Client
            {
                ClientId = "m2m.postman",
                ClientName = "Postman Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                AllowedScopes = { "sportsynchro.api.read", "sportsynchro.api.write" }
            },
            // frontend client using code flow 
            new Client {
                ClientId = "webapp-client",
                RequireClientSecret = false,
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireConsent = false,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "roles",
                    "sportsynchro.api.read",
                    "sportsynchro.api.write"
                },
                RedirectUris = { "http://localhost:5173/auth/callback" },
                PostLogoutRedirectUris = { "http://localhost:5173/" },
                AllowedCorsOrigins = ["http://localhost:5173"],
                AllowAccessTokensViaBrowser = true
            }
        };
}
