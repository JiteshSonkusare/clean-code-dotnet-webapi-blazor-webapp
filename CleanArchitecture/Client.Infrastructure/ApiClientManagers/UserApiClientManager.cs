using System.Net;
using Client.Infrastructure.Routes;
using Client.Infrastructure.Client;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Configuration;
using Client.Infrastructure.Security.Interfaces;
using Client.Infrastructure.Contracts;

namespace Client.Infrastructure.ApiClientManagers
{
    public class UserApiClientManager : ApiClientBase
    {
        public UserApiClientManager(IClientConfig config, IAuthHandler authHandler) : base(config, authHandler)
        {
            if (string.IsNullOrWhiteSpace(config?.BaseUrl))
            {
                throw new ArgumentNullException(nameof(config.BaseUrl));
            }
        }

        public async Task<Response<UserResponse>?> GetAllAsync()
        {
            ResponseData result = await Send(
                    new Uri(UserEndpoints.GetAll, UriKind.Relative),
                    HttpMethod.Get,
                    null,
                     CancellationToken.None,
                    Array.Empty<HeaderData>()
                ).ConfigureAwait(false);

            if (result.StatusCode == (int)HttpStatusCode.NotFound)
                return new Response<UserResponse>(new ResponseData(HttpStatusCode.NotFound, Array.Empty<UserResponse>().ToJson(), result.ResponseHeaders), HttpStatusCode.NotFound);
            if (result.StatusCode == (int)HttpStatusCode.OK)
                return new Response<UserResponse>(result, result.StatusCode);

            throw new GeneralApplicationException(result.Content);

        }

        protected override DefaultRequestHeaders GetDefaultRequestHeaders()
        {
            return new DefaultRequestHeaders(new string[] { System.Net.Mime.MediaTypeNames.Application.Json }, Array.Empty<HeaderData>());
        }
    }
}
