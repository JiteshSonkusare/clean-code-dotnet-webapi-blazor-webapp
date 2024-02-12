using Microsoft.Identity.Web;
using System.Security.Authentication;
using Client.Infrastructure.Security.Interfaces;
using Client.Infrastructure.Security.ViewModels;
using Client.Infrastructure.Security.Configurations;

namespace Client.Infrastructure.Security.AuthHandlers.AzureAD
{
    public sealed class AzureAdAuthHandler : IAuthHandler
    {
        private readonly AuthConfig _config;
        private readonly ITokenAcquisition _tokenAcquisition;

        public AzureAdAuthHandler(AuthConfig config, ITokenAcquisition tokenAcquisition)
        {
            _config = config;
            _tokenAcquisition = tokenAcquisition;
        }

        public async Task<IAuthToken> GetAuthToken(CancellationToken cancellation)
        {
            try
            {
                long requestedAt = DateTime.Now.Ticks;
                var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { _config.Scopes });
                var token = new TokenResponse
                {
                    Access_Token = accessToken,
                    Token_Type   = "Bearer",
                };
                if (string.IsNullOrWhiteSpace(token?.Access_Token))
                    throw new AuthenticationException("An empty/null token value received.");

                token.RequestedAt = requestedAt;
                return token;
            }
            catch (Exception ex)
            {
                throw new AuthenticationException("Error occurred while executing get token for user!", ex);
            }
        }
    }
}