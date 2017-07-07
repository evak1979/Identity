using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SocialNetwork.OAuth.Startup))]

namespace SocialNetwork.OAuth
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Security.Claims;
    using System.Security.Cryptography.X509Certificates;
    using IdentityServer3.Core;
    using IdentityServer3.Core.Configuration;
    using IdentityServer3.Core.Models;
    using IdentityServer3.Core.Services.InMemory;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var factory = new IdentityServerServiceFactory()
                .UseInMemoryUsers(InMemoryManager.GetUsers())
                .UseInMemoryScopes(InMemoryManager.GetScopes())
                .UseInMemoryClients(InMemoryManager.GetClients());
                
            var certificate = Convert.FromBase64String(ConfigurationManager.AppSettings["SigningCertificate"]);

            var options = new IdentityServerOptions()
            {
                SigningCertificate = new X509Certificate2(certificate, ConfigurationManager.AppSettings["SigningCertificatePassword"]),
                RequireSsl = false,
                Factory = factory
            };

            app.UseIdentityServer(options);
        }
    }

    public static class InMemoryManager
    {
        public static List<InMemoryUser> GetUsers()
        {
            return new List<InMemoryUser>()
            {
                new InMemoryUser()
                {
                    Subject = "eaktoudianakis@gmail.com",
                    Username = "eaktoudianakis@gmail.com",
                    Password = "password",
                    Claims = new List<Claim>()
                    {
                        new Claim(Constants.ClaimTypes.Name, "Evan Aktoudianakis")
                    }
                }
            };   
        }

        public static IEnumerable<Scope> GetScopes()
        {
            return new[]
            {
                StandardScopes.OpenId,
                StandardScopes.Profile,
                StandardScopes.OfflineAccess,
                new Scope
                {
                    Name = "read",
                    DisplayName = "Read User Data"
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client()
                {
                    ClientId = "socialnetwork",
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret("secret".Sha256())
                    },
                    Flow = Flows.ResourceOwner,
                    AllowedScopes = new List<string>()
                    {
                        Constants.StandardScopes.OpenId,
                        "read",
                        Constants.StandardScopes.Profile
                    },
                    Enabled = true
                }
            };
        }
    }
}
