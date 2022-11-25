namespace Microsoft.Extensions.Caching.Memory;

public static class MemoryCacheExtensions
{
    public static async Task<T?> GetOrCreateWithExpirationAsync<T>(this IMemoryCache cache, object key, Func<Task<(T?, TimeSpan)>> factory)
        where T : class
    {
        if (!cache.TryGetValue(key, out T? result))
        {
            (result, TimeSpan expiration) = await factory();

            if (result != null)
            {
                cache.CreateEntry(key).SetSlidingExpiration(expiration).SetValue(result);
            }
        }

        return result;
    }
}
