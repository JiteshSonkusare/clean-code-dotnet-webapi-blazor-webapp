namespace Client.Infrastructure.Security.Interfaces
{
    public interface IAuthHandler
    {
        Task<IAuthToken> GetAuthToken(CancellationToken cancellation);
    }
}
