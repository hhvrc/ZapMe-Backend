using Quartz;
using ZapMe.Attributes;
using ZapMe.Services.Interfaces;

namespace ZapMe.Jobs;

/// <summary>
/// Cleans up expired password requests
/// Runs once every hour
/// </summary>
[QuartzTimer("CleanupEmailVerificationRequests", QuartzTimer.Predefined.Hourly)]
public sealed class CleanupEmailVerificationRequests : IJob
{
    private readonly IEmailVerificationManager _emailVerificationManager;
    private readonly ILogger<CleanupEmailVerificationRequests> _logger;

    public CleanupEmailVerificationRequests(IEmailVerificationManager emailVerificationManager, ILogger<CleanupEmailVerificationRequests> logger)
    {
        _emailVerificationManager = emailVerificationManager;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        int nRemoved = await _emailVerificationManager.RemoveExpiredRequestsAsync();
        if (nRemoved > 0)
            _logger.LogInformation("Removed {nRemoved} email verification request entities", nRemoved);
    }
}