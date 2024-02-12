namespace Client.Infrastructure.Client
{
    public sealed class DefaultRequestHeaders
    {
        public DefaultRequestHeaders(string[] acceptedMediaTypes, params HeaderData[] headers)
        {
            AcceptedMediaTypes = acceptedMediaTypes;
            Headers = headers;
        }

        internal string[] AcceptedMediaTypes { get; }
        internal IEnumerable<HeaderData> Headers { get; }
    }
}