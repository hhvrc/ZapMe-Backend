using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Http;

public static class IHeaderDictionaryExtensions
{
    public static string? GetFirst(this IHeaderDictionary headers, string name)
    {
        if (headers.TryGetValue(name, out StringValues values))
        {
            return values.FirstOrDefault();
        }

        return null;
    }
    public static StringValues GetPrefferedHeader(this IHeaderDictionary headers, params string[] names)
    {
        foreach (string name in names)
        {
            if (headers.TryGetValue(name, out StringValues value) && value.Any())
            {
                return value;
            }
        }

        return StringValues.Empty;
    }
}
