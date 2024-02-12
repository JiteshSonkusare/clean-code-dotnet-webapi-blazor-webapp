namespace AuthorizationHandler.Lib.Configurations
{
    public class GenesysAuthOptions
    {
        public string GenesysConfigWebServiceUri { get; set; } = null!;
        public int CacheInMinutes                { get; set; } = 2;
    }
}