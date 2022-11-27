using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using ZapMe.DTOs;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class HybridCache : IHybridCache
{
    public IMemoryCache MemoryCache { get; }
    public IDistributedCache DistributedCache { get; }
    private readonly ILogger<HybridCache> _logger;

    public HybridCache(IMemoryCache memoryCache, IDistributedCache distrubutedCache, ILogger<HybridCache> logger)
    {
        MemoryCache = memoryCache;
        DistributedCache = distrubutedCache;
        _logger = logger;
    }

    private void SetMemoryCache<T>(string key, HybridCacheEntry<T> entry)
    {
        ICacheEntry memcacheEntry = MemoryCache.CreateEntry(key);
        memcacheEntry.SlidingExpiration = entry.ExpiresAtUtc - DateTime.UtcNow;
        memcacheEntry.Value = entry.Value;
    }

    public async Task SetAsync<T>(string key, HybridCacheEntry<T> entry, CancellationToken cancellationToken)
    {
        SetMemoryCache(key, entry);
        await DistributedCache.SetStringAsync(key, JsonSerializer.Serialize(entry), cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan lifetime, CancellationToken cancellationToken)
    {
        HybridCacheEntry<T> entry = new HybridCacheEntry<T>
        {
            Value = value,
            ExpiresAtUtc = DateTime.UtcNow + lifetime
        };
        await SetAsync(key, entry, cancellationToken);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        if (MemoryCache.TryGetValue(key, out HybridCacheEntry<T> cacheEntry))
        {
            if (cacheEntry.ExpiresAtUtc > DateTime.UtcNow)
            {
                return cacheEntry.Value;
            }
        }

        string? distrubutedCacheResult = await DistributedCache.GetStringAsync(key, cancellationToken);
        if (distrubutedCacheResult != null)
        {
            try
            {
                cacheEntry = JsonSerializer.Deserialize<HybridCacheEntry<T>>(distrubutedCacheResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize cache entry");
                await DistributedCache.RemoveAsync(key, cancellationToken);
                return default;
            }

            if (cacheEntry.ExpiresAtUtc > DateTime.UtcNow)
            {
                SetMemoryCache(key, cacheEntry);

                return cacheEntry.Value;
            }
        }

        return default;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        return DistributedCache.RemoveAsync(key, cancellationToken);
    }

    public async Task<T> GetOrAddAsync<T>(string key, Func<string, HybridCacheEntry<T>> factory, CancellationToken cancellationToken = default)
    {
        T? entry = await GetAsync<T>(key, cancellationToken);

        if (entry == null)
        {
            HybridCacheEntry<T> value = factory(key);
            await SetAsync(key, value, cancellationToken);
            entry = value.Value;
        }

        return entry;
    }

    public async Task<T> GetOrAddAsync<T>(string key, Func<string, CancellationToken, Task<HybridCacheEntry<T>>> factory, CancellationToken cancellationToken = default)
    {
        T? entry = await GetAsync<T>(key, cancellationToken);

        if (entry == null)
        {
            HybridCacheEntry<T> value = await factory(key, cancellationToken);
            await SetAsync(key, value, cancellationToken);
            entry = value.Value;
        }

        return entry;
    }
}