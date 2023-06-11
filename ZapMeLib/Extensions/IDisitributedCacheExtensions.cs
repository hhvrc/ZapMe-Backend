using System.Text.Json;

namespace Microsoft.Extensions.Caching.Distributed;

public static class IDisitributedCacheExtensions
{
    public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, CancellationToken cancellationToken = default)
    {
        await cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(value), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
        }, cancellationToken);
    }

    public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
    {
        byte[]? data = await cache.GetAsync(key, cancellationToken);

        if (data is null)
            return default;

        return JsonSerializer.Deserialize<T>(data);
    }

    public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key, T defaultValue, CancellationToken cancellationToken = default)
    {
        byte[]? data = await cache.GetAsync(key, cancellationToken);

        if (data is null)
            return defaultValue;

        return JsonSerializer.Deserialize<T>(data) ?? defaultValue;
    }
}
