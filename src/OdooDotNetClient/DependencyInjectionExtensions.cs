using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OdooDotNetClient
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddOdooClient(this IServiceCollection services, IConfiguration configuration)
        {
            // Register options
            services.Configure<OdooOptions>(configuration);

            // Register HttpClient + OdooClient implementation
            services.AddHttpClient<IOdooClient, OdooClient>();

            return services;
        }
        
        public static IServiceCollection AddOdooClient(this IServiceCollection services, Action<OdooOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddHttpClient<IOdooClient, OdooClient>();
            return services;
        }
    }
}
