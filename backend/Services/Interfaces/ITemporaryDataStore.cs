namespace ZapMe.Services.Interfaces;

public interface ITemporaryDataStore
{
    Task SetAsync(string key, string value, DateTime expiresAtUtc, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, DateTime expiresAtUtc, CancellationToken cancellationToken = default) where T : class;
    Task<string?> GetAsync(string key, CancellationToken cancellationToken = default);
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task<int> CleanupExpiredData(CancellationToken cancellationToken = default);
}