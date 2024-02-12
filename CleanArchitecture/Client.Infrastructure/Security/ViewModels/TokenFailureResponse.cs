namespace Client.Infrastructure.Security.ViewModels
{
    internal sealed class TokenFailureResponse
    {
        public string Error_Description { get; set; }
        
        public string Error { get; set; }

        public override string ToString() => $"{Error_Description}({Error}).";
    }
}
