using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityProvider.WebApi.Config;

public static class KestrelConfiguration
{
    public static IServiceCollection ConfigureKestrel(this IServiceCollection services)
    {
        services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
        {
            options.ListenAnyIP(44319, options => options.UseHttps());
        });

        return services;
    }
}
