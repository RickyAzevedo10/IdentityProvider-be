using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace IdentityProvider.Infrastructure.Identity;

/// <summary>
/// OpenIddict core configuration (DbContext, EF Core, Quartz).
/// </summary>
public static class OpenIddictConfiguration
{
    /// <summary>
    /// Adds OpenIddict core services (DbContext, EF Core, Quartz) to the DI container.
    /// Server and validation configuration must be added separately in the Server project.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOpenIddictCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(options =>
        {
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.AddDbContext<DbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                    sqlOptions.CommandTimeout(30);
                });
            options.UseOpenIddict();
        });

        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                       .UseDbContext<DbContext>();
            });

        return services;
    }
}
