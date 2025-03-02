namespace CC.Auth.Api.Constants.v1;

public static class AppConfiguration
{
    public static class Sections
    {
        public const string Cosmos = nameof(Cosmos);
        public const string CosmosAuth = $"{Cosmos}:Auth";
    }

    public static class Keys
    {
        public const string AppConfigConnectionString = $"AppConfig:ConnectionString";
        public const string CosmosConnectionString = $"{Sections.Cosmos}:ConnectionString";
        public const string CosmosDatabaseId = $"{Sections.CosmosAuth}:DatabaseId";
    }
}