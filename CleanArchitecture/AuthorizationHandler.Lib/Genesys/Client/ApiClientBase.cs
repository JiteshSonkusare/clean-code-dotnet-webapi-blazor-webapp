using System.Net;
using System.Net.Mime;
using AuthorizationHandler.Lib.Exceptions;
using AuthorizationHandler.Lib.Configurations;

namespace AuthorizationHandler.Lib.Genesys.Client
{
    public abstract class ApiClientBase : IDisposable
    {
        private readonly HttpClient _client;
        private readonly GenesysAuthOptions _options;

        public ApiClientBase(GenesysAuthOptions options)
        {
            _options = options;
            _client = new HttpClient() 
            { 
                BaseAddress = new Uri(_options.GenesysConfigWebServiceUri),
                Timeout     = new TimeSpan(0, 0, 30) 
            };
            _client.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
        }

        protected async Task Send(Uri requestUri, HttpMethod method, Action<(HttpStatusCode statusCode, string content)> responseParser, CancellationToken cancellation)
        {
            var message = new HttpRequestMessage(method, requestUri);
            var result = await ExecuteRequest(message, cancellation);
            responseParser.Invoke(result);
        }

        private async Task<(HttpStatusCode statusCode, string content)> ExecuteRequest(HttpRequestMessage request, CancellationToken cancellation)
        {
            try
            {
                HttpResponseMessage? response = null;
                Exception? taskError = null;
                await _client.SendAsync(request, cancellation).ContinueWith(task =>
                {
                    if (task.Exception != null)
                        taskError = task.Exception;
                    else
                        response = task.Result;
                }, cancellation);
                if (taskError != null)
                    throw taskError;
                if (response == null)
                    throw new Exception("Unknown error! Could not retrieve response.");

                string content = string.Empty;
                await response.Content.ReadAsStringAsync(cancellation).ContinueWith(task =>
                {
                    if (task.Exception != null)
                        taskError = task.Exception;
                    else
                        content = task.Result;
                }, cancellation);
                if (taskError != null)
                    throw taskError;

                return (response.StatusCode, content);
            }
            catch (Exception ex)
            {
                throw new GeneralApplicationException("Error occurred while executing HTTP request.", ex);
            }
        }

        void IDisposable.Dispose() => _client.Dispose();
    }
}
