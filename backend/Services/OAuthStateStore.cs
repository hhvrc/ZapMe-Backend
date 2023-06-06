using Microsoft.Extensions.Caching.Distributed;
using ZapMe.Authentication;
using ZapMe.Constants;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;


public sealed class OAuthStateStore : IOAuthStateStore
{
    private const string _CacheKeyStatePrefix = "OAuthState:";
    private const string _CacheKeyRegistrationTicketPrefix = "OAuthRegistrationTicket:";
    private sealed record InternalStateData<T>(string ProviderName, string RequestIP, T Data);
    private sealed record InternalTicketData<T>(string RequestIP, T Data);

    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<OAuthStateStore> _logger;

    public OAuthStateStore(IDistributedCache distributedCache, ILogger<OAuthStateStore> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public sealed record StateData(string RedirectUri);
    public async Task<string> CreateStateAsync(string providerName, string requestIP, string redirectUri, CancellationToken cancellationToken)
    {
        string requestKey = StringUtils.GenerateUrlSafeRandomString(32);

        var stateInternal = new InternalStateData<StateData>(providerName, requestIP, new StateData(redirectUri));
        await _distributedCache.SetAsync(_CacheKeyStatePrefix + HashingUtils.Sha256_Base64(requestKey), stateInternal, OAuthConstants.StateLifetime, cancellationToken);

        return requestKey;
    }

    public async Task<StateData?> GetStateAsync(string requestKey, string providerName, string requestIP, CancellationToken cancellationToken)
    {
        var stateInternal = await _distributedCache.GetAsync<InternalStateData<StateData>>(_CacheKeyStatePrefix + HashingUtils.Sha256_Base64(requestKey), cancellationToken);

        if (stateInternal == null || stateInternal.ProviderName != providerName || stateInternal.RequestIP != requestIP)
        {
            _logger.LogWarning("OAuth state for request key {RequestKey} was not found, or the provider name or request IP did not match", requestKey);
            return null;
        }

        return stateInternal.Data;
    }

    public async Task<string> CreateRegistrationTicketAsync(string requestIP, OAuthProviderVariables providerVariables, CancellationToken cancellationToken)
    {
        string requestKey = StringUtils.GenerateUrlSafeRandomString(32);

        var stateInternal = new InternalTicketData<OAuthProviderVariables>(requestIP, providerVariables);
        await _distributedCache.SetAsync(_CacheKeyRegistrationTicketPrefix + HashingUtils.Sha256_Base64(requestKey), stateInternal, OAuthConstants.RegistrationTicketLifetime, cancellationToken);

        return requestKey;
    }

    public async Task<OAuthProviderVariables?> GetRegistrationTicketAsync(string requestKey, string requestIP, CancellationToken cancellationToken)
    {
        var stateInternal = await _distributedCache.GetAsync<InternalTicketData<OAuthProviderVariables>>(_CacheKeyRegistrationTicketPrefix + HashingUtils.Sha256_Base64(requestKey), cancellationToken);

        if (stateInternal == null || stateInternal.RequestIP != requestIP)
        {
            _logger.LogWarning("OAuth registration ticket for request key {RequestKey} was not found, or the provider name or request IP did not match", requestKey);
            return null;
        }

        return stateInternal.Data;
    }
}
