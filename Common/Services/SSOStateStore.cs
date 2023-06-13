using Microsoft.Extensions.Caching.Distributed;
using ZapMe.Constants;
using ZapMe.DTOs;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;


public sealed class SSOStateStore : ISSOStateStore
{
    private sealed record InternalData<T>(string ProviderName, string RequestIP, T Data);

    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<SSOStateStore> _logger;

    public SSOStateStore(IDistributedCache distributedCache, ILogger<SSOStateStore> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    private static string GetStateCacheKey(string requestKey) => RedisCachePrefixes.SSOState + HashingUtils.Sha256_Base64(requestKey);

    public async Task<string> CreateStateAsync(string providerName, string requestIP, SSOStateData stateData, CancellationToken cancellationToken)
    {
        string requestKey = StringUtils.GenerateUrlSafeRandomString(32);

        var stateInternal = new InternalData<SSOStateDataEntry>(providerName, requestIP, new SSOStateDataEntry(stateData.RedirectUrl, DateTime.UtcNow + SSOConstants.StateLifetime));
        await _distributedCache.SetAsync(GetStateCacheKey(requestKey), stateInternal, SSOConstants.StateLifetime, cancellationToken);

        return requestKey;
    }

    public async Task<SSOStateDataEntry?> GetStateAsync(string requestKey, string providerName, string requestIP, CancellationToken cancellationToken)
    {
        var stateInternal = await _distributedCache.GetAsync<InternalData<SSOStateDataEntry>>(GetStateCacheKey(requestKey), cancellationToken);

        if (stateInternal is null || stateInternal.ProviderName != providerName || stateInternal.RequestIP != requestIP)
        {
            return null;
        }

        return stateInternal.Data;
    }

    private static string GetProviderDataCacheKey(string requestKey) => RedisCachePrefixes.SSOProviderData + HashingUtils.Sha256_Base64(requestKey);

    public async Task<string> InsertProviderDataAsync(string requestIP, SSOProviderData providerData, CancellationToken cancellationToken)
    {
        string requestKey = StringUtils.GenerateUrlSafeRandomString(32);

        var stateInternal = new InternalData<SSOProviderDataEntry>(providerData.ProviderName, requestIP, new SSOProviderDataEntry(providerData.ProviderName, providerData.ProviderUserId, providerData.ProviderUserName, providerData.ProviderUserEmail, providerData.ProviderUserEmailVerified, providerData.ProfilePictureUrl, DateTime.UtcNow + SSOConstants.RegistrationTicketLifetime));
        await _distributedCache.SetAsync(GetProviderDataCacheKey(requestKey), stateInternal, SSOConstants.RegistrationTicketLifetime, cancellationToken);

        return requestKey;
    }

    public async Task<SSOProviderDataEntry?> GetProviderDataAsync(string requestKey, string requestIP, CancellationToken cancellationToken)
    {
        var stateInternal = await _distributedCache.GetAsync<InternalData<SSOProviderDataEntry>>(GetProviderDataCacheKey(requestKey), cancellationToken);

        if (stateInternal is null || stateInternal.RequestIP != requestIP)
        {
            return null;
        }

        return stateInternal.Data;
    }
}
