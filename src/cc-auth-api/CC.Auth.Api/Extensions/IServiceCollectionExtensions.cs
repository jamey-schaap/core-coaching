namespace CC.Auth.Api.Extensions;

public static class AppConfig
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

public class OptionsBase(string sectionName)
{
    public string SectionName { get; init; } = sectionName;
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValidatableOptions<TOptions>(this IServiceCollection services, List<Func<TOptions, bool>>? validators = null)
        where TOptions : OptionsBase
    {
        var sectionName = typeof(TOptions)
            .GetField("SectionName")!
            .GetValue(null) as string;

        ArgumentNullException.ThrowIfNull(sectionName);

        var optionsBuilder = services
            .AddOptions<TOptions>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        validators?.ForEach(v => optionsBuilder.Validate(v));

        return services;
    }
}