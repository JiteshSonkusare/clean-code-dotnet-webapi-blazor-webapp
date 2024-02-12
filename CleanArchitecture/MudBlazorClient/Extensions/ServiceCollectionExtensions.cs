using MudBlazor;
using MudBlazor.Services;
using MudBlazorClient.Data;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace MudBlazorClient.Extensions
{
    public static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddAzureAuthDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            if (!configuration.GetValue<bool>("AuthConfig:IsAuthRequired"))
                services.AddRazorPages();
            else
            {
                var initialScopes = configuration["AuthConfig:Scopes"]?.Split(' ') ?? configuration["MicrosoftGraph:Scopes"]?.Split(' ');
                services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApp(configuration.GetSection("AzureAd"))
                        .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                            .AddDownstreamWebApi("ApiClientConfig", configuration.GetSection("ApiClientConfig"))
                            .AddInMemoryTokenCaches();
                services.AddRazorPages().AddMvcOptions(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                                  .RequireAuthenticatedUser()
                                  .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                }).AddMicrosoftIdentityUI();

                services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Events.OnSignedOutCallbackRedirect = async context =>
                    {
                        context.HttpContext.Response.Redirect("/");
                        context.HandleResponse();
                    };
                });
            }

            return services;
        }

		internal static IServiceCollection RegiterDI(this IServiceCollection services)
        {
			services.AddSingleton<WeatherForecastService>();
			return services;
        }

        internal static IServiceCollection AddMudBlazorServices(this IServiceCollection services)
        {
			services.AddMudServices(configuration =>
			{
				configuration.SnackbarConfiguration.PositionClass          = Defaults.Classes.Position.BottomRight;
				configuration.SnackbarConfiguration.HideTransitionDuration = 100;
				configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
				configuration.SnackbarConfiguration.VisibleStateDuration   = 3000;
				configuration.SnackbarConfiguration.ShowCloseIcon          = false;
			});
            return services;
		}
	}
}
