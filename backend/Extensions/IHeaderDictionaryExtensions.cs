using AngleSharp.Io;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;
using System.Xml.Linq;

namespace Microsoft.AspNetCore.Http;

public static class IHeaderDictionaryExtensions
{
    public static string? GetPrefferedHeader(this IHeaderDictionary headers, params string[] names)
    {
        foreach (string name in names)
        {
            if (headers.TryGetValue(name, out StringValues values) && values.Any())
            {
                return values.ToString();
            }
        }

        return null;
    }
}
