using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using IdentityProvider.Application.Interfaces;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityProvider.Infrastructure.Data;

/// <summary>
/// Seeds OpenIddict applications and scopes into the database.
/// </summary>
public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<DbContext>();
        await context.Database.EnsureCreatedAsync();

        await CreateApplicationsAsync(scope.ServiceProvider);
        await CreateScopesAsync(scope.ServiceProvider);
    }

    private static async Task CreateApplicationsAsync(IServiceProvider serviceProvider)
    {
        var manager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        await CreateReactAppAsync(manager);
    }

    private static async Task CreateReactAppAsync(IOpenIddictApplicationManager manager)
    {
        var existing = await manager.FindByClientIdAsync("react_app");
        if (existing is not null)
        {
            await manager.DeleteAsync(existing);
        }

        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "react_app",
            ClientType = ClientTypes.Public,
            RedirectUris =
            {
                new Uri("http://localhost:3000/callback"),
                new Uri("http://localhost:5173/callback")
            },
            PostLogoutRedirectUris =
            {
                new Uri("http://localhost:3000"),
                new Uri("http://localhost:5173")
            },
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Prefixes.Scope + "api1"
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            }
        });
    }

    private static async Task CreateScopesAsync(IServiceProvider serviceProvider)
    {
        var manager = serviceProvider.GetRequiredService<IOpenIddictScopeManager>();

        if (await manager.FindByNameAsync("api1") is null)
        {
            await manager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "api1",
                Resources = { "zirku_api" }
            });
        }
    }
}
