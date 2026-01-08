using System.Security.Claims;
using IdentityModel;
using SportSynchro.IdentityServer.Data;
using SportSynchro.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;

namespace SportSynchro.IdentityServer;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure admin role exists
            if (!roleMgr.RoleExistsAsync("admin").Result)
            {
                var roleResult = roleMgr.CreateAsync(new IdentityRole("admin")).Result;
                if (!roleResult.Succeeded)
                {
                    throw new Exception(roleResult.Errors.First().Description);
                }

                Log.Debug("admin role created");
            }
            else
            {
                Log.Debug("admin role already exists");
            }

            // Ensure user role exists
            if (!roleMgr.RoleExistsAsync("user").Result)
            {
                var roleResult = roleMgr.CreateAsync(new IdentityRole("user")).Result;
                if (!roleResult.Succeeded)
                {
                    throw new Exception(roleResult.Errors.First().Description);
                }

                Log.Debug("user role created");
            }
            else
            {
                Log.Debug("user role already exists");
            }

            var alice = userMgr.FindByNameAsync("alice").Result;
            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                };
                var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(alice, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, "Alice Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Alice"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                Log.Debug("alice created");
            }
            else
            {
                Log.Debug("alice already exists");
            }

            if (!userMgr.IsInRoleAsync(alice, "admin").Result)
            {
                var addRoleResult = userMgr.AddToRoleAsync(alice, "admin").Result;
                if (!addRoleResult.Succeeded)
                {
                    throw new Exception(addRoleResult.Errors.First().Description);
                }

                Log.Debug("alice added to admin role");
            }
            else
            {
                Log.Debug("alice already in admin role");
            }

            var bob = userMgr.FindByNameAsync("bob").Result;
            if (bob == null)
            {
                bob = new ApplicationUser
                {
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true
                };
                var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(bob, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, "Bob Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Bob"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                    new Claim("location", "somewhere")
                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                Log.Debug("bob created");
            }
            else
            {
                Log.Debug("bob already exists");
            }

            if (!userMgr.IsInRoleAsync(bob, "user").Result)
            {
                var addRoleResult = userMgr.AddToRoleAsync(bob, "user").Result;
                if (!addRoleResult.Succeeded)
                {
                    throw new Exception(addRoleResult.Errors.First().Description);
                }

                Log.Debug("bob added to user role");
            }
            else
            {
                Log.Debug("bob already in user role");
            }

        }

        using (IServiceScope scope = app.Services
                   .GetRequiredService<IServiceScopeFactory>()
                   .CreateScope())
        {
            ConfigurationDbContext context = scope.ServiceProvider
                .GetRequiredService<ConfigurationDbContext>();

            IdentityServerConfigFactory factory = scope.ServiceProvider
                .GetRequiredService<IdentityServerConfigFactory>();

            Log.Debug("Seeding IdentityServer configuration");

            // Clients
            if (!context.Clients.Any())
            {
                foreach (Client client in factory.GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }

                context.SaveChanges();
                Log.Debug("Clients seeded");
            }

            // Identity Resources
            if (!context.IdentityResources.Any())
            {
                foreach (IdentityResource resource in factory.GetIdentityResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
                Log.Debug("IdentityResources seeded");
            }

            // Api Scopes
            if (!context.ApiScopes.Any())
            {
                foreach (ApiScope scopeDef in factory.GetApiScopes())
                {
                    context.ApiScopes.Add(scopeDef.ToEntity());
                }

                context.SaveChanges();
                Log.Debug("ApiScopes seeded");
            }

            // Api Resources
            if (context.ApiResources.Any()) return;
            foreach (ApiResource api in factory.GetApiResources())
            {
                context.ApiResources.Add(api.ToEntity());
            }

            context.SaveChanges();
            Log.Debug("ApiResources seeded");
        }
    }
}
