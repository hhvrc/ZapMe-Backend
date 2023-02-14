using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UAParser;
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

        UserAgentEntity? entry = await _dbContext.UserAgents.Where(x => x.Hash == hash).FirstOrDefaultAsync(cancellationToken);
        if (entry != null)
        {
            return entry;
        }

        string parsedOS, parsedDevice, parsedUserAgent;
        try
        {
            ClientInfo parsed = Parser.GetDefault().Parse(userAgent);
            parsedOS = parsed.OS.ToString();
            parsedDevice = parsed.Device.ToString();
            parsedUserAgent = parsed.UA.ToString();
        }
        catch (Exception)
        {
            parsedOS = parsedDevice = parsedUserAgent = "Unknown (Parsing failed)";
        }

        if (userAgent.Length > UserAgentLimits.StoredLength)
        {
            userAgent = userAgent[..UserAgentLimits.StoredLength];
        }

        entry = new UserAgentEntity
        {
            Hash = hash,
            Length = length,
            Value = userAgent,
            ParsedOperatingSystem = parsedOS,
            ParsedDevice = parsedDevice,
            ParsedUserAgent = parsedUserAgent
        };

        await _dbContext.UserAgents.AddAsync(entry, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entry;
    }
}