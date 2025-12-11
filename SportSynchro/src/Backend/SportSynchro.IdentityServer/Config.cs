using Duende.IdentityServer.Models;

namespace SportSynchro.IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
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
            // m2m client credentials flow client
            new Client
            {
                ClientId = "m2m.postman",
                ClientName = "Postman Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                AllowedScopes = { "sportsynchro.api.read", "sportsynchro.api.write" }
            },
        };
}
