using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace CC.Common.Configuration;

public static class ConfigurationManagerExtensions
{
    public static IConfigurationManager ValidateKeys(this IConfigurationManager configuration, params (string Key, Type type)[] keysAndTypes)
    {
        List<string> nullOrEmptyKeys = [];
        foreach (var (key, type) in keysAndTypes)
        {
            var value = configuration.GetValue(type, key, null);
            if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                nullOrEmptyKeys.Add(key);
            }
        }

        if (nullOrEmptyKeys.Count != 0)
        {
            throw new ArgumentException("The value of one or more keys is null, empty or consists only of white-space characters.")
            {
                Data =
                {
                    [nameof(nullOrEmptyKeys)] = nullOrEmptyKeys
                }
            };
        }

        return configuration;
    }

    public static IConfigurationManager ValidateAllKeys(this IConfigurationManager configuration, Type staticKeysType)
    {
        var keysAndTypes = staticKeysType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(field => (key: (string)field.GetValue(null)!, type: field.FieldType))
            .ToArray();

        return configuration.ValidateKeys(keysAndTypes);
    }
}
