using ZapMe.Authentication;

namespace ZapMe.Services.Interfaces;

public interface ISSOStateStore
{
    Task<string> CreateStateAsync(string providerName, string requestIP, string redirectUri, CancellationToken cancellationToken = default);
    Task<SSOStateStore.StateData?> GetStateAsync(string requestKey, string providerName, string requestIP, CancellationToken cancellationToken = default);
    Task<string> CreateRegistrationTokenAsync(string requestIP, OAuthProviderVariables providerVariables, CancellationToken cancellationToken = default);
    Task<OAuthProviderVariables?> GetRegistrationTokenAsync(string requestKey, string requestIP, CancellationToken cancellationToken = default);
}