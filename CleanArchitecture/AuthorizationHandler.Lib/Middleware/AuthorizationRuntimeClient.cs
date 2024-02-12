using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using AuthorizationHandler.Lib.Configurations;
using AuthorizationHandler.Lib.Genesys.Client;

namespace AuthorizationHandler.Lib.Middleware
{
    public class AuthorizationRuntimeClient : IAuthorizationRuntimeClient
    {
        private readonly IMemoryCache _cache;
        private readonly IOptions<GenesysAuthOptions> _options;

        public AuthorizationRuntimeClient(IOptions<GenesysAuthOptions> options, IMemoryCache cache)
        {
            _cache = cache;
            _options = options;
        }

        public async Task<bool> ResolveAuthAsync(ClaimsPrincipal user, CancellationToken cancellation)
        {
            var clientId = user.Claims.FirstOrDefault(c => c.Type.Equals("appid"))?.Value;
            var employeeId = user.Claims.FirstOrDefault(c => c.Type.Equals("employeeid"))?.Value;

            if (string.IsNullOrEmpty(clientId))
                throw new UnauthorizedAccessException("ClientId not found!");
            if (string.IsNullOrEmpty(employeeId))
                throw new UnauthorizedAccessException("EmployeeId not found!");

            var genesysClaims = await GetClaimsFromGenesysConfigOrCache(employeeId, clientId, cancellation);
            if (genesysClaims.Count > 0)
            {
                var claims = new List<Claim>();

                foreach (var claim in genesysClaims)
                    claims.Add(new Claim(ClaimTypes.Role, claim));

                claims.Add(new Claim("clientid", clientId));
                claims.Add(new Claim("employeeid", employeeId));

                var id = new ClaimsIdentity(claims);
                user.AddIdentity(id);
            }
            return true;
        }

        private async Task<List<string>> GetClaimsFromGenesysConfigOrCache(string employeeId, string clientId, CancellationToken cancellation)
        {
            string cacheKey = clientId + "_" + employeeId;
            List<string> claimsFromGenessysConfig;
            if (_options.Value.CacheInMinutes > 0 && _cache.TryGetValue<List<string>>(cacheKey, out var cacheEntry))
            {
                claimsFromGenessysConfig = cacheEntry;
            }
            else
            {
                claimsFromGenessysConfig = new List<string>();
                var client = new GenesysConfigServiceApiClient(_options.Value);
                var roles = await client.GetRolesFromGenesysConfig(employeeId, cancellation);
                if (roles != null)
                    claimsFromGenessysConfig.AddRange(roles);

                if (_options.Value.CacheInMinutes > 0)
                {
                    cacheEntry = claimsFromGenessysConfig;
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(_options.Value.CacheInMinutes));

                    _cache.Set(cacheKey, cacheEntry, cacheEntryOptions);
                }
            }
            return claimsFromGenessysConfig;
        }
    }
}
