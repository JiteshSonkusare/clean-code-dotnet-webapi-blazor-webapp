using System.Net;
using Newtonsoft.Json;
using AuthorizationHandler.Lib.Exceptions;
using AuthorizationHandler.Lib.Configurations;

namespace AuthorizationHandler.Lib.Genesys.Client
{
    public class GenesysConfigServiceApiClient : ApiClientBase
    {
        private readonly GenesysAuthOptions _options;

        public GenesysConfigServiceApiClient(GenesysAuthOptions options) : base(options)
        {
            _options = options;
        }

        public async Task<IList<string>?> GetRolesFromGenesysConfig(string employeeId, CancellationToken cancellation)
        {
            IList<string>? response = null;
            var endpoint = new Uri($"{_options.GenesysConfigWebServiceUri}/{employeeId}", UriKind.Absolute);
            try
            {
                await Send(endpoint,
                         HttpMethod.Get,
                         result => response = result.statusCode == 
                            HttpStatusCode.OK
                            ? JsonConvert.DeserializeObject<IList<string>?>(result.content)
                            : null,
                         cancellation);

                return response;
            }
            catch (Exception ex)
            {
                throw new GeneralApplicationException("Get roles from genesys failed!", ex);
            }
        }
    }
}