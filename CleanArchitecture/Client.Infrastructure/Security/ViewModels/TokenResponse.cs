using Client.Infrastructure.Security.Interfaces;
using System.Text.Json.Serialization;

namespace Client.Infrastructure.Security.ViewModels
{
    internal sealed class TokenResponse : IAuthToken
    {
        [JsonPropertyName("access_token")]
        public string Access_Token { get; set; } = string.Empty;

        [JsonPropertyName("token_type")]
        public string Token_Type { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int Expires_In { get; set; }

        [JsonIgnore]
        string IAuthToken.Scheme { get => Token_Type; set => Token_Type = value; }

        [JsonIgnore]
        string IAuthToken.Value { get => Access_Token; set => Access_Token = value; }
        
        [JsonIgnore]
        public long RequestedAt { get; set; }
        
        [JsonIgnore]
        int? IAuthToken.RefreshAfter { get => Expires_In * 1000; set => Expires_In = (value ?? 0) / 1000; }
    }
}
