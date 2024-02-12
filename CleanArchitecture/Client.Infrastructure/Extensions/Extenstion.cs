using Newtonsoft.Json;

namespace Client.Infrastructure.Extensions
{
    public static class Extenstion
    {
        private static readonly JsonSerializer serializer;

        static Extenstion()
        {
            serializer = JsonSerializer.CreateDefault();
            serializer.NullValueHandling = NullValueHandling.Ignore;
        }

        public static string ToJson(this object obj)
        {
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, obj);
            return writer.ToString();
        }
    }
}