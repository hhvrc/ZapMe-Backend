using Quartz;
using ZapMe.Attributes;
using ZapMe.Services.Interfaces;

namespace ZapMe.Jobs;

/// <summary>
/// Cleans up expired password requests
/// Runs once every 10 minutes
/// </summary>
[QuartzTimer("CleanupPasswordResets", 10, QuartzTimer.Interval.Minute)]
public sealed class CleanupPasswordResets : IJob
{
    private readonly IPasswordResetRequestManager _passwordResetRequestManager;
    private readonly ILogger<WeeklyJob> _logger;

    public CleanupPasswordResets(IPasswordResetRequestManager passwordResetRequestManager, ILogger<WeeklyJob> logger)
    {
        _passwordResetRequestManager = passwordResetRequestManager;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Cleaning up expired password reset requests...");
        int nRemoved = await _passwordResetRequestManager.RemoveExpiredRequests();
        _logger.LogInformation("Removed {nRemoved} reset request entities", nRemoved);
    }
}