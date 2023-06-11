using ZapMe.DTOs;

namespace ZapMe.Services.Interfaces;

public interface ISSOStateStore
{
    Task<string> CreateStateAsync(string providerName, string requestIP, SSOStateData stateData, CancellationToken cancellationToken = default);
    Task<SSOStateDataEntry?> GetStateAsync(string requestKey, string providerName, string requestIP, CancellationToken cancellationToken = default);
    Task<string> InsertProviderDataAsync(string requestIP, SSOProviderData providerData, CancellationToken cancellationToken = default);
    Task<SSOProviderDataEntry?> GetProviderDataAsync(string requestKey, string requestIP, CancellationToken cancellationToken = default);
}