using Microsoft.AspNetCore.Authentication;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace ZapMe.Helpers;

public class DistributedCacheSecureDataFormat<T> : ISecureDataFormat<T>
{
    private static readonly ConcurrentDictionary<string, byte[]> _KeyValuePairs = new ConcurrentDictionary<string, byte[]>();
    /*
     * TODO: implement
     * IMPLEMENTME
    private readonly IDistributedCache _cache;

    public DistributedCacheSecureDataFormat(IDistributedCache cache)
    {
        _cache = cache;
    }
    */
    public string Protect(T data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
        string key = Utils.StringUtils.GenerateUrlSafeRandomString(32);
        _KeyValuePairs.TryAdd(key, bytes);
        return key;
    }

    public string Protect(T data, string? purpose)
    {
        return Protect(data);
    }

    public T? Unprotect(string? protectedText)
    {
        if (protectedText == null)
        {
            return default;
        }

        if (!_KeyValuePairs.TryGetValue(protectedText, out byte[]? bytes) || bytes == null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(bytes));
    }

    public T? Unprotect(string? protectedText, string? purpose)
    {
        return Unprotect(protectedText);
    }
}
