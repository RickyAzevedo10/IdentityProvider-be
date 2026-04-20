using Microsoft.Extensions.DependencyInjection;

namespace IdentityProvider.Server.Config;

public static class CorsConfiguration
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options => options.AddDefaultPolicy(policy =>
            policy.AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
                  .WithOrigins(
                      "http://localhost:3000",
                      "http://localhost:5173")));

        return services;
    }
}
