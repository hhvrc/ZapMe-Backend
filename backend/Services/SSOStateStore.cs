using Microsoft.Extensions.Caching.Distributed;
using ZapMe.Constants;
using ZapMe.DTOs;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;


public sealed class SSOStateStore : ISSOStateStore
{
    private const string _CacheKeyStatePrefix = "SSOState:";
    private const string _CacheKeyRegistrationTokenPrefix = "SSOORegistrationToken:";
    private sealed record InternalStateData<T>(string ProviderName, string RequestIP, T Data);
    private sealed record InternalTokenData<T>(string RequestIP, T Data);

    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<SSOStateStore> _logger;

    public SSOStateStore(IDistributedCache distributedCache, ILogger<SSOStateStore> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<string> CreateStateAsync(string providerName, string requestIP, SSOStateData stateData, CancellationToken cancellationToken)
    {
        string requestKey = StringUtils.GenerateUrlSafeRandomString(32);

        var stateInternal = new InternalStateData<SSOStateDataEntry>(providerName, requestIP, new SSOStateDataEntry(stateData.RedirectUrl, DateTime.UtcNow + SSOConstants.StateLifetime));
        await _distributedCache.SetAsync(_CacheKeyStatePrefix + HashingUtils.Sha256_Base64(requestKey), stateInternal, SSOConstants.StateLifetime, cancellationToken);

        return requestKey;
    }

    public async Task<SSOStateDataEntry?> GetStateAsync(string requestKey, string providerName, string requestIP, CancellationToken cancellationToken)
    {
        var stateInternal = await _distributedCache.GetAsync<InternalStateData<SSOStateDataEntry>>(_CacheKeyStatePrefix + HashingUtils.Sha256_Base64(requestKey), cancellationToken);

        if (stateInternal == null || stateInternal.ProviderName != providerName || stateInternal.RequestIP != requestIP)
        {
            _logger.LogWarning("SSO state for request key {RequestKey} was not found, or the provider name or request IP did not match", requestKey);
            return null;
        }

        return stateInternal.Data;
    }

    public async Task<string> CreateRegistrationTokenAsync(string requestIP, SSOProviderData providerData, CancellationToken cancellationToken)
    {
        string requestKey = StringUtils.GenerateUrlSafeRandomString(32);

        var stateInternal = new InternalTokenData<SSOProviderDataEntry>(requestIP, new SSOProviderDataEntry(providerData.ProviderName, providerData.ProviderUserId, providerData.ProviderUserName, providerData.ProviderUserEmail, providerData.ProviderUserEmailVerified, providerData.ProfilePictureUrl, DateTime.UtcNow + SSOConstants.RegistrationTicketLifetime));
        await _distributedCache.SetAsync(_CacheKeyRegistrationTokenPrefix + HashingUtils.Sha256_Base64(requestKey), stateInternal, SSOConstants.RegistrationTicketLifetime, cancellationToken);

        return requestKey;
    }

    public async Task<SSOProviderDataEntry?> GetRegistrationTokenAsync(string requestKey, string requestIP, CancellationToken cancellationToken)
    {
        var stateInternal = await _distributedCache.GetAsync<InternalTokenData<SSOProviderDataEntry>>(_CacheKeyRegistrationTokenPrefix + HashingUtils.Sha256_Base64(requestKey), cancellationToken);

        if (stateInternal == null || stateInternal.RequestIP != requestIP)
        {
            _logger.LogWarning("SSO registration token for request key {RequestKey} was not found, or the provider name or request IP did not match", requestKey);
            return null;
        }

        return stateInternal.Data;
    }
}
