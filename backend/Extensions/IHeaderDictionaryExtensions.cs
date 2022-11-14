using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Http;

public static class IHeaderDictionaryExtensions
{
    public static StringValues GetPrefferedHeader(this IHeaderDictionary headers, params string[] names)
    {
        foreach (var name in names)
        {
            if (headers.TryGetValue(name, out var value) && value.Any())
            {
                return value;
            }
        }

        return StringValues.Empty;
    }
}
