using ZapMe.Authentication;

namespace ZapMe.Services.Interfaces;

public interface IOAuthStateStore
{
    Task<string> CreateStateAsync(string providerName, string requestIP, string redirectUri, CancellationToken cancellationToken = default);
    Task<OAuthStateStore.StateData?> GetStateAsync(string requestKey, string providerName, string requestIP, CancellationToken cancellationToken = default);
    Task<string> CreateRegistrationTicketAsync(string requestIP, OAuthProviderVariables providerVariables, CancellationToken cancellationToken = default);
    Task<OAuthProviderVariables?> GetRegistrationTicketAsync(string requestKey, string requestIP, CancellationToken cancellationToken = default);
}