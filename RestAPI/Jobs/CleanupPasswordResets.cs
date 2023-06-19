using Quartz;
using ZapMe.Attributes;
using ZapMe.Services.Interfaces;

namespace ZapMe.Jobs;

/// <summary>
/// Cleans up expired password requests
/// Runs once every hour
/// </summary>
[QuartzTimer("CleanupPasswordResets", QuartzTimer.Predefined.Hourly)]
public sealed class CleanupPasswordResets : IJob
{
    private readonly IPasswordResetManager _passwordResetManager;
    private readonly ILogger<CleanupPasswordResets> _logger;

    public CleanupPasswordResets(IPasswordResetManager passwordResetManager, ILogger<CleanupPasswordResets> logger)
    {
        _passwordResetManager = passwordResetManager;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        int nRemoved = await _passwordResetManager.RemoveExpiredRequests();
        if (nRemoved > 0)
            _logger.LogInformation("Removed {nRemoved} password reset request entities", nRemoved);
    }
}