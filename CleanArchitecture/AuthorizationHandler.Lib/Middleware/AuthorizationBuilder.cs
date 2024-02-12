using Microsoft.Extensions.DependencyInjection;

namespace AuthorizationHandler.Lib.Middleware
{
    public class AuthorizationBuilder
    {
        public IServiceCollection Services { get; }

        public AuthorizationBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
