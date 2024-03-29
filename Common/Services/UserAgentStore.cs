﻿using ZapMe.Constants;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class UserAgentStore : IUserAgentStore
{
    private readonly DatabaseContext _dbContext;

    public UserAgentStore(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserAgentEntity> CreateAsync(string sha256, uint length, string value, string operatingSystem, string device, string browser, CancellationToken cancellationToken)
    {
        if (sha256.Length != HashConstants.Sha256LengthHex) throw new ArgumentException($"Hash should be {HashConstants.Sha256LengthHex} characters", nameof(sha256));

        UserAgentEntity userAgent = new UserAgentEntity
        {
            Sha256 = sha256,
            Length = length,
            Value = value,
            OperatingSystem = operatingSystem,
            Device = device,
            Browser = browser
        };

        _dbContext.UserAgents.Add(userAgent);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return userAgent;
    }
}