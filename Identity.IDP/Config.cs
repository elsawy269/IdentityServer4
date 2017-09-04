using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.IDP
{
    public static class Config
    {
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser{
                    SubjectId="121252-52255-55555",
                    Username="mohamed",
                    Password="password",
                    Claims=new List<Claim>
                    {
                        new Claim("given_name","Mohamed"),
                        new Claim("family_name","El sawy"),
                        new Claim("address","1, MainRoad"),
                        new Claim("role","freeUser"),
                        new Claim("country","egypt"),
                        new Claim("subscriptionlevel","freeUser")
                        
                    }
                },
                 new TestUser{
                    SubjectId="121252-52255-65656",
                    Username="reda",
                    Password="password",
                    Claims=new List<Claim>
                    {
                        new Claim("given_name","Reda"),
                        new Claim("family_name","Kamal"),
                        new Claim("address","2, Big Street"),
                        new Claim("role","payingUser"),
                           new Claim("country","ksa"),
                        new Claim("subscriptionlevel","payingUser")

                    }
                },
            };

        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
               new IdentityResources.OpenId(),
               new IdentityResources.Profile(),
               new IdentityResources.Address(),
               new IdentityResource("roles","Your role(s)",new List<string>(){"role" }),
               new IdentityResource("country","the country where you live",
               new List<string>(){"country" }),
               new IdentityResource("subscriptionlevel","Your subscription level",
               new List<string>(){"subscriptionlevel" }),

            };
        }

        public static IEnumerable<ApiResource> GetApiResource()
        {
            return new List<ApiResource>
            {
                new ApiResource("UserManagementAPI","User Management API",new List<string>(){"role" })
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client> {
                new Client {


                    ClientName="Admin UIMVC",
                    ClientId="adminMVC",
                    AllowedGrantTypes=GrantTypes.Hybrid,
                    //  IdentityTokenLifetime=300,
                    AccessTokenLifetime=120,
                    //  AbsoluteRefreshTokenLifetime

                    //RefreshTokenExpiration=TokenExpiration.Sliding;
                    //SlidingRefreshTokenLifetime=

                    UpdateAccessTokenClaimsOnRefresh=true,
                    AllowOfflineAccess=true,


                    RedirectUris =new List<string>
                    {
                        "https://localhost:44306/signin-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "UserManagementAPI",
                        "country",
                        "subscriptionlevel",
                        IdentityServerConstants.StandardScopes.OfflineAccess

                    },
                    ClientSecrets =
                    {
                        new Secret("secrect".Sha256())
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:44306/signout-callback-oidc"
                    }
                    //AlwaysIncludeUserClaimsInIdToken=true
                },

            };
        }
    }
}
