using System.Security.Claims;

namespace AuthorizationHandler.Lib.Middleware
{
    public interface IAuthorizationRuntimeClient
    {
        Task<bool> ResolveAuthAsync(ClaimsPrincipal user, CancellationToken cancellation);
    }
}
