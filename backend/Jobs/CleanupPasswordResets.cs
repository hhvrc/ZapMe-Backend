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
    private readonly IPasswordResetManager _passwordResetManager;
    private readonly ILogger<WeeklyJob> _logger;

    public CleanupPasswordResets(IPasswordResetManager passwordResetManager, ILogger<WeeklyJob> logger)
    {
        _passwordResetManager = passwordResetManager;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Cleaning up expired password reset requests...");
        int nRemoved = await _passwordResetManager.RemoveExpiredRequests();
        _logger.LogInformation("Removed {nRemoved} reset request entities", nRemoved);
    }
}