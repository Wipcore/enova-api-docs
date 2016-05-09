using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Core;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services.InMemory;

namespace Wipcore.Enova.Api.OAuth
{
    public class InMemoryManager
    {
        public static List<InMemoryUser> GetUsers()
        {
            //return new List<InMemoryUser>()
            //{
            //    new InMemoryUser()
            //    {
            //        Subject = "testsson", Password = "test", Username = "test", Claims = new { new Claim(Constants.ClaimTypes.)}
            //    }
            //};

            var users = new List<InMemoryUser>
            {
                new InMemoryUser
                {
                    Subject = "818727",
                    Username = "alice",
                    Password = "alice",
                    Claims = new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.Role, "Admin"),
                        new Claim(JwtClaimTypes.Role, "Geek"),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new Claim(JwtClaimTypes.Address,
                            @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }",
                            Constants.ClaimValueTypes.Json)
                    }
                }
            };
            return users;
        }

        public static List<Scope> GetScopes()
        {
            return new List<Scope>()
            {
                //StandardScopes.OpenId,
                //StandardScopes.Profile,
                StandardScopes.OfflineAccess,
                new Scope
                {
                    Name = "customer",
                    DisplayName = "Customer API Access",
                    Description = "A customer can read most data and update itself, carts, addresses and orders",
                    Type = ScopeType.Resource,
                    ScopeSecrets = new List<Secret>
                    {
                        new Secret("customersecret".Sha256())
                    }
                    //},
                    //Claims = new List<ScopeClaim>
                    //{
                    //    new ScopeClaim("role")
                    //}
                },
                new Scope()
                {
                    Name = "integrator",
                    DisplayName = "Integration API Access",
                    Description = "An integrator can read, update and delete most types of data",
                    Type = ScopeType.Resource,
                    ScopeSecrets = new List<Secret>
                    {
                        new Secret("integratorsecret".Sha256())
                    }
                }
            };
        }

        public static List<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client()
                {
                    ClientId = "netclient",
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret("net".Sha256())
                    },
                    Flow = Flows.ResourceOwner,
                    AllowedScopes = new List<string>() { "customer", "integrator", StandardScopes.OfflineAccess.Name  }, //TODO JS client 
                    Enabled = true
                }
            };
        } 
    }
}
