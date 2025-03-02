using Microsoft.Extensions.DependencyInjection;

namespace CC.Common.Configuration.Extensions;

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
            .GetValue(obj: null) as string;

        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);

        var optionsBuilder = services
            .AddOptions<TOptions>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        validators?.ForEach(v => optionsBuilder.Validate(v));

        return services;
    }
}
