using System.Diagnostics.CodeAnalysis;

namespace ZapMe.Extensions;

public static class IConfigurationExtensions
{
    public static string GetOrThrow([NotNull] this IConfiguration configuration, string key)
    {
        string? value = configuration[key];
        if (value is null)
        {
            throw new InvalidOperationException($"Configuration key '{key}' is missing.");
        }

        return value;
    }
}
