using Microsoft.Extensions.Configuration;
using AuthorizationHandler.Lib.Middleware;
using AuthorizationHandler.Lib.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuthorizationHandler.Lib.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static AuthorizationBuilder AddGenesysAuthMiddleware(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAuthorizationRuntimeClient, AuthorizationRuntimeClient>();
            services.Configure<GenesysAuthOptions>(configuration.GetSection(nameof(GenesysAuthOptions)));
            services.AddMemoryCache();

            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer           = false,
                    ValidateAudience         = false,
                };
                options.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = async context =>
                    {
                        IAuthorizationRuntimeClient authClient = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationRuntimeClient>();
                        if (context.Principal != null)
                            await authClient.ResolveAuthAsync(context.Principal, CancellationToken.None);
                    }
                };
            });

            return new AuthorizationBuilder(services);
        }
    }
}
