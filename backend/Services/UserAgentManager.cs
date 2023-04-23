using UAParser;
using ZapMe.Constants;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;

public sealed class UserAgentManager : IUserAgentManager
{
    public IUserAgentStore Store { get; }
    private readonly ILogger<UserAgentManager> _logger;

    public UserAgentManager(IUserAgentStore store, ILogger<UserAgentManager> logger)
    {
        Store = store;
        _logger = logger;
    }

    public async Task<UserAgentEntity> EnsureCreatedAsync(string userAgent, CancellationToken cancellationToken = default)
    {
        uint length = (uint)userAgent.Length;
        string sha256 = HashingUtils.Sha256_Hex(userAgent);

        UserAgentEntity? entry = await Store.GetByHashAsync(sha256, cancellationToken);
        if (entry != null) return entry;

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
            _logger.LogWarning(ex, "Failed to parse user agent {UserAgent}", userAgent);
            os = "Unknown";
            device = "Unknown";
            browser = "Unknown";
        }

        if (userAgent.Length > UserAgentLimits.StoredValueLength)
        {
            userAgent = userAgent[..UserAgentLimits.StoredValueLength];
        }

        return await Store.CreateAsync(sha256, length, userAgent, os, device, browser, cancellationToken);
    }
}