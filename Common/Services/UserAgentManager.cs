using Microsoft.EntityFrameworkCore;
using UAParser;
using ZapMe.Constants;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;

public sealed class UserAgentManager : IUserAgentManager
{
    private readonly DatabaseContext _dbContext;
    private readonly IUserAgentStore _userAgentStore;
    private readonly ILogger<UserAgentManager> _logger;

    public UserAgentManager(DatabaseContext dbContext, IUserAgentStore userAgentStore, ILogger<UserAgentManager> logger)
    {
        _dbContext = dbContext;
        _userAgentStore = userAgentStore;
        _logger = logger;
    }

    public async Task<UserAgentEntity> EnsureCreatedAsync(string userAgent, CancellationToken cancellationToken)
    {
        uint length = (uint)userAgent.Length;
        string sha256 = HashingUtils.Sha256_Hex(userAgent);

        UserAgentEntity? entry = await _dbContext.UserAgents.FirstOrDefaultAsync(u => u.Sha256 == sha256, cancellationToken);
        if (entry is not null)
        {
            return entry;
        }

        string os;
        string device;
        string browser;
        try
        {
            ClientInfo clientInfo = Parser.GetDefault().Parse(userAgent);
            os = clientInfo.OS.ToString();
            device = clientInfo.Device.ToString();
            browser = clientInfo.UA.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse user agent with hash [{hash}], inserting into database with fields set to \"unknown\"", sha256);
            os = "Unknown";
            device = "Unknown";
            browser = "Unknown";
        }

        if (userAgent.Length > UserAgentLimits.StoredValueLength)
        {
            userAgent = userAgent[..UserAgentLimits.StoredValueLength];
        }

        return await _userAgentStore.CreateAsync(sha256, length, userAgent, os, device, browser, cancellationToken);
    }
}