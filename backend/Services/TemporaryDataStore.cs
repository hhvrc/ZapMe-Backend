using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class TemporaryDataStore : ITemporaryDataStore
{
    private readonly ZapMeContext _dbContext;
    private readonly ILogger<TemporaryDataStore> _logger;

    public TemporaryDataStore(ZapMeContext dbContext, ILogger<TemporaryDataStore> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SetAsync(string key, string value, DateTime expiresAtUtc, CancellationToken cancellationToken = default)
    {
        var entry = new TemporaryStringDataEntity
        {
            Key = key,
            Value = value,
            ExpiresAt = expiresAtUtc
        };

        await _dbContext.TemporaryStringData.AddAsync(entry, cancellationToken);
        await _dbContext.SaveChangesAsync();
    }
    public async Task SetAsync<T>(string key, T value, DateTime expiresAtUtc, CancellationToken cancellationToken = default) where T : class
    {
        var entry = new TemporaryJsonDataEntity
        { 
            Key = key,
            Value = JsonSerializer.Serialize(value),
            ExpiresAt = expiresAtUtc
        };

        await _dbContext.TemporaryJsonData.AddAsync(entry, cancellationToken);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        TemporaryStringDataEntity? entry = await _dbContext.TemporaryStringData.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
        if (entry == null)
        {
            return null;
        }
        if (entry.ExpiresAt < DateTime.UtcNow)
        {
            try
            {
                _dbContext.TemporaryStringData.Remove(entry);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
            }
            return null;
        }
        return entry.Value;
    }
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        TemporaryJsonDataEntity? entry = await _dbContext.TemporaryJsonData.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
        if (entry == null)
        {
            return null;
        }
        if (entry.ExpiresAt < DateTime.UtcNow)
        {
            try
            {
                _dbContext.TemporaryJsonData.Remove(entry);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
            }
            return null;
        }
        return JsonSerializer.Deserialize<T>(entry.Value);
    }

    public async Task<int> CleanupExpiredData(CancellationToken cancellationToken = default)
    {
        int nRemoved = 0;
        nRemoved += await _dbContext.TemporaryJsonData.Where(s => s.ExpiresAt < DateTime.UtcNow).ExecuteDeleteAsync(cancellationToken);
        nRemoved += await _dbContext.TemporaryJsonData.Where(s => s.ExpiresAt < DateTime.UtcNow).ExecuteDeleteAsync(cancellationToken);
        return nRemoved;
    }
}
