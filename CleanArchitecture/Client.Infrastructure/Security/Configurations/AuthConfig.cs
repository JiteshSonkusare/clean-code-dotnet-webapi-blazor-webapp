using Client.Infrastructure.Configuration;

namespace Client.Infrastructure.Security.Configurations
{
    public class AuthConfig : IClientConfig
    {
        public string? BaseUrl { get; set; }

        public TimeSpan? Timeout { get; set; }

        public string Scopes { get; set; } = null!;

        public bool IsAuthRequired { get; set; }
    }
}
