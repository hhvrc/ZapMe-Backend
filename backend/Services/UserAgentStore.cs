using System.Security.Cryptography;
using System.Text;
using ZapMe.Constants;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class UserAgentStore : IUserAgentStore
{
    private readonly ZapMeContext _dbContext;

    public UserAgentStore(ZapMeContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserAgentEntity> EnsureCreatedAsync(string userAgent, CancellationToken cancellationToken = default)
    {
        int length = userAgent.Length;
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(userAgent));

        if (userAgent.Length > UserAgentLimits.StoredLength)
        {
            userAgent = userAgent[..UserAgentLimits.StoredLength];
        }

        var entry = new UserAgentEntity
        {
            Hash = hash,
            Length = length,
            Value = userAgent
        };

        await _dbContext.UserAgents.AddAsync(entry, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entry;
    }
}