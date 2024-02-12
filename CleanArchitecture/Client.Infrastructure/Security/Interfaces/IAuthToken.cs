namespace Client.Infrastructure.Security.Interfaces
{
    public interface IAuthToken
    {
        string Scheme { get; set; }

        string Value { get; set; }

        long RequestedAt { get; set; }

        int? RefreshAfter { get; set; }
    }
}
