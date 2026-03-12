using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Cloudflare.EmailRouting.Abstract;
using Soenneker.Cloudflare.Utils.Client.Registrars;

namespace Soenneker.Cloudflare.EmailRouting.Registrars;

/// <summary>
/// A utility for managing Cloudflare email routing and addresses
/// </summary>
public static class CloudflareEmailRoutingUtilRegistrar
{
    /// <summary>
    /// Adds <see cref="ICloudflareEmailRoutingUtil"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddCloudflareEmailRoutingUtilAsSingleton(this IServiceCollection services)
    {
        services.AddCloudflareClientUtilAsSingleton().TryAddSingleton<ICloudflareEmailRoutingUtil, CloudflareEmailRoutingUtil>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="ICloudflareEmailRoutingUtil"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddCloudflareEmailRoutingUtilAsScoped(this IServiceCollection services)
    {
        services.AddCloudflareClientUtilAsSingleton().TryAddScoped<ICloudflareEmailRoutingUtil, CloudflareEmailRoutingUtil>();

        return services;
    }
}
