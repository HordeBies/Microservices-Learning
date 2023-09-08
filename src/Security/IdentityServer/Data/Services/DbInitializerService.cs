using IdentityModel;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityServer.Data.Services
{
    public class DbInitializerService
    {
        private readonly PersistedGrantDbContext persistedGrantDbContext;
        private readonly ConfigurationDbContext configContext;
        private readonly ApplicationDbContext applicationContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DbInitializerService(PersistedGrantDbContext persistedGrantDbContext, ConfigurationDbContext configContext, ApplicationDbContext applicationContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.persistedGrantDbContext = persistedGrantDbContext ?? throw new ArgumentNullException(nameof(persistedGrantDbContext));
            this.configContext = configContext ?? throw new ArgumentNullException(nameof(configContext));
            this.applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public async Task Initialize()
        {
            // Configure IS4
            persistedGrantDbContext.Database.Migrate();
            configContext.Database.Migrate();

            await SeedIs4Configs();

            // Configure Identity
            applicationContext.Database.Migrate();

            await SeedRoles();

            await SeedUsers();
        }

        private async Task SeedIs4Configs()
        {
            if (!configContext.Clients.Any())
            {
                foreach (var client in Config.Clients)
                {
                    configContext.Clients.Add(client.ToEntity());
                }
                await configContext.SaveChangesAsync();
            }

            if (!configContext.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    configContext.IdentityResources.Add(resource.ToEntity());
                }
                await configContext.SaveChangesAsync();
            }

            if (!configContext.ApiScopes.Any())
            {
                foreach (var scope in Config.ApiScopes)
                {
                    configContext.ApiScopes.Add(scope.ToEntity());
                }
                await configContext.SaveChangesAsync();
            }

            if (!configContext.ApiResources.Any())
            {
                foreach (var resource in Config.ApiResources)
                {
                    configContext.ApiResources.Add(resource.ToEntity());
                }
                await configContext.SaveChangesAsync();
            }
        }

        private async Task SeedRoles()
        {

            //if(!roleManager.Roles.Any())
            //{
            //    await roleManager.CreateAsync(new IdentityRole("Admin"));
            //    await roleManager.CreateAsync(new IdentityRole("User"));
            //}
            foreach (var role in AccountOptions.AllowedRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private async Task SeedUsers()
        {
            var alice = await userManager.FindByNameAsync("alice");
            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true
                };
            var result = await userManager.CreateAsync(alice, "Pass123$");
            if(!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
            result = await userManager.AddClaimsAsync(alice, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, "Alice Smith"),
                new Claim(JwtClaimTypes.GivenName, "Alice"),
                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
            });
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
            result = await userManager.AddToRoleAsync(alice, "user");
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
                //Log.Debug("alice created");
            }
            else
            {
                //Log.Debug("alice already exists");
            }

            var bob = await userManager.FindByNameAsync("bob");
            if (bob == null)
            {
                bob = new ApplicationUser
                {
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(bob, "Pass123$");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                result = await userManager.AddClaimsAsync(bob, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, "Bob Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Bob"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                    new Claim("location", "somewhere")
                });
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                result = await userManager.AddToRoleAsync(bob, "admin");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                //Log.Debug("bob created");
            }
            else
            {
                //Log.Debug("bob already exists");
            }
        }
    }
}
