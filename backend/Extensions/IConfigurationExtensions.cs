using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.Configuration;

public static class IConfigurationExtensions
{
    public static string GetOrThrow([NotNull] this IConfiguration configuration, string key)
    {
        string? value = configuration[key];
        if (value == null)
        {
            throw new InvalidOperationException($"Configuration key '{key}' is missing.");
        }

        return value;
    }
}
