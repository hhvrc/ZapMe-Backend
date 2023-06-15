using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.Caching.Distributed;

public static class IDisitributedCacheExtensions
{
    private static readonly JsonSerializerOptions _SerializerOptions = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };

    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, CancellationToken cancellationToken = default) =>
        cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(value, _SerializerOptions), cancellationToken);

    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default) =>
        cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(value, _SerializerOptions), options, cancellationToken);

    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, DateTime absoluteExpiration, CancellationToken cancellationToken = default) =>
        SetAsync(cache, key, value, new DistributedCacheEntryOptions { AbsoluteExpiration = absoluteExpiration }, cancellationToken);

    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, TimeSpan absoluteExpirationRelativeToNow, CancellationToken cancellationToken = default) =>
        SetAsync(cache, key, value, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow }, cancellationToken);

    public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
    {
        byte[]? data = await cache.GetAsync(key, cancellationToken);

        if (data is null)
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(data);
        }
        catch (Exception)
        {
            return default;
        }
    }

    public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key, T defaultValue, CancellationToken cancellationToken = default)
    {
        byte[]? data = await cache.GetAsync(key, cancellationToken);

        if (data is null)
            return defaultValue;

        return JsonSerializer.Deserialize<T>(data) ?? defaultValue;
    }
}
