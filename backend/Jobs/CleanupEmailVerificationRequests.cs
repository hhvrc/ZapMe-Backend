using Quartz;
using ZapMe.Attributes;
using ZapMe.Services.Interfaces;

namespace ZapMe.Jobs;

/// <summary>
/// Cleans up expired password requests
/// Runs once every 10 minutes
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
        _logger.LogInformation("Cleaning up expired email verification requests...");
        int nRemoved = await _emailVerificationManager.RemoveExpiredRequestsAsync();
        _logger.LogInformation("Removed {nRemoved} email verification request entities", nRemoved);
    }
}