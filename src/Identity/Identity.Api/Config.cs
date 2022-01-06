namespace Identity.Api;

using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResources.Phone(),
            new IdentityResources.Address(),
            new IdentityResource("roles", "User Roles", new List<string> { "role" })
        };


    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("shop-api", "Shop Web API")
        };

    public static List<ApiResource> ApiResources =>

        new List<ApiResource>
        {
            new ApiResource("ShopApiResource", "Shop Web API Resource")
            {
                Scopes = { "shop-api" },
                UserClaims = {
                    JwtClaimTypes.Role,
                    JwtClaimTypes.Name,
                    JwtClaimTypes.Id
                }
            }
        };


    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new Client
            {
                ClientId = "client",
                ClientName = "React Client",
                RequireClientSecret = false,
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "roles",
                    "shop-api"
                }
            },
            new Client
            {
                ClientId = "oidcClient",
                ClientName = "Example Client Application",
                ClientSecrets = new List<Secret> {new Secret("SuperSecretPassword".Sha256())}, // change me!

                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = new List<string> {"https://localhost:7001/signin-oidc"},
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "role",
                    "shop-api"
                },

                RequirePkce = true,
                AllowPlainTextPkce = false
            }
        };
}