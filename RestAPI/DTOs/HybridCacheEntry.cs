namespace ZapMe.DTOs;

public struct HybridCacheEntry<T>
{
    /// <summary>
    /// 
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime ExpiresAtUtc { get; set; }
}
