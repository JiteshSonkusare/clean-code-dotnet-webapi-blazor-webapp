using System.Diagnostics;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Configuration;
using Client.Infrastructure.Security.Interfaces;
using Client.Infrastructure.Security.Extensions;

namespace Client.Infrastructure.Client
{
    public abstract class ApiClientBase : IDisposable
    {
        private bool disposedValue;
        private readonly IClientConfig _config;
        private readonly IAuthHandler? _authHandler;
        private readonly SocketsHttpHandler httpHandler;

        protected ApiClientBase(IClientConfig config, IAuthHandler? authHandler)
        {
            _config = config;
            _authHandler = authHandler;
            httpHandler = new SocketsHttpHandler()
            {
                UseCookies                  = false,
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
                PooledConnectionLifetime    = TimeSpan.FromSeconds(55),
                KeepAlivePingPolicy         = HttpKeepAlivePingPolicy.Always,
                KeepAlivePingDelay          = TimeSpan.FromSeconds(2),
                KeepAlivePingTimeout        = TimeSpan.FromSeconds(1)
            };
        }

        private HttpClient CreateClient()
        {
            HttpClient client = new HttpClient(httpHandler, false)
            {
                Timeout = _config?.Timeout ?? TimeSpan.FromSeconds(15)
            };

            if (!string.IsNullOrWhiteSpace(_config?.BaseUrl))
                client.BaseAddress = new Uri(_config.BaseUrl);

            DefaultRequestHeaders defaultRequestHeaders = GetDefaultRequestHeaders();
            if (defaultRequestHeaders?.AcceptedMediaTypes?.Length > 0)
            {
                foreach (string mediaType in defaultRequestHeaders.AcceptedMediaTypes)
                    client.DefaultRequestHeaders?.Accept.ParseAdd(mediaType);
            }
            if (defaultRequestHeaders?.Headers?.Count() > 0)
            {
                foreach (HeaderData header in defaultRequestHeaders.Headers)
                    client.DefaultRequestHeaders?.Add(header.Name, header.Values);
            }
            return client;
        }

        protected abstract DefaultRequestHeaders GetDefaultRequestHeaders();

        protected async Task<ResponseData> Send(Uri requestUri, HttpMethod method, HttpContent? requestContent, CancellationToken cancellation, params HeaderData[]? requestHeaders)
        {
            try
            {
                using HttpClient client = CreateClient();
                if (_authHandler != null)
                {
                    var token = await _authHandler.GetAuthToken(cancellation).ConfigureAwait(false);
                    client.DefaultRequestHeaders.Authorization = token.GetAuthorizationHeader();
                }
                using HttpRequestMessage request = new HttpRequestMessage(method, requestUri);
                if (requestContent != null)
                    request.Content = requestContent;
                if (requestHeaders != null)
                    foreach (var header in requestHeaders)
                        request.Headers.Add(header.Name, header.Values);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                using HttpResponseMessage response = await client.SendAsync(request, cancellation).ConfigureAwait(false);
                
                string responseContent = await response.Content.ReadAsStringAsync(cancellation).ConfigureAwait(false);
                stopwatch.Stop();

                return new ResponseData(response.StatusCode, responseContent, response.Headers, response.TrailingHeaders);
            }
            catch (Exception ex)
            {
                //Logger.Error(ex, "Error occurred while executing HTTP request.");
                throw new GeneralApplicationException($"Failed to call api in ApiClientBase", ex);
            }
        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                httpHandler.Dispose();
                disposedValue = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
