using Microsoft.Extensions.Configuration;
using Client.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Client.Infrastructure.Security.Configurations;

namespace Client.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClientInfrastrctureDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiClientConfig>(configuration.GetSection(nameof(ApiClientConfig)));
            services.Configure<AuthConfig>(configuration.GetSection(nameof(AuthConfig)));
            return services;
        }
    }
}