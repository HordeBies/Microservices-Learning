using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<Client> Clients => new List<Client>()
        {
            new Client
            {
                ClientId = "mvc_local",
                ClientName = "Local MVC Web App Client",
                AllowedGrantTypes = GrantTypes.Hybrid,
                RequirePkce = false,
                AllowOfflineAccess = true,
                RedirectUris = new List<string>()
                {
                    "http://localhost:5006/signin-oidc"
                },
                PostLogoutRedirectUris = new List<string>()
                {
                    "http://localhost:5006/signout-callback-oidc"
                },
                ClientSecrets = new List<Secret>
                {
                    new Secret("3112208D-D517-4239-A87D-E761FA8E39CC".Sha256())
                },
                AllowedScopes = new List<string>()
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Address,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    "api_rwx"
                }
            },
            new Client
            {
                ClientId= "mvc_docker",
                ClientName = "Docker MVC Web App Client",
                AllowedGrantTypes = GrantTypes.Hybrid,
                RequirePkce = false,
                AllowOfflineAccess = true,
                RedirectUris = new List<string>()
                {
                    "http://localhost:8006/signin-oidc"
                },
                PostLogoutRedirectUris = new List<string>()
                {
                    "http://localhost:8006/signout-callback-oidc"
                },
                ClientSecrets = new List<Secret>
                {
                    new Secret("3112208D-D517-4239-A87D-E761FA8E39CC".Sha256())
                },
                AllowedScopes = new List<string>()
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Address,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    "api_rwx"
                }
            }
        };

        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>()
        {
            new ApiScope("api_rwx", "Api Read/Write/Change", userClaims: new[] { "role" })
        };

        public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>()
        {

        };

        public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>()
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Address(),
            new IdentityResources.Email(),
            new IdentityResource("roles","Your role(s)", new List<string>(){ "role" })
        };
    }
}
