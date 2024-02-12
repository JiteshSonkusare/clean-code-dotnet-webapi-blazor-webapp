using System.Net.Http.Headers;
using Client.Infrastructure.Security.Interfaces;

namespace Client.Infrastructure.Security.Extensions
{
    internal static class AuthExtensions
    {
        public static AuthenticationHeaderValue GetAuthorizationHeader(this IAuthToken token)
            => new AuthenticationHeaderValue(token.Scheme, token.Value);
    }
}