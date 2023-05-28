using Quartz;
using ZapMe.Attributes;
using ZapMe.Services.Interfaces;

namespace ZapMe.Jobs;

/// <summary>
/// Cleans up expired temporary data
/// Runs once a minute
/// </summary>
[QuartzTimer("CleanupTemporaryData", 1, QuartzTimer.Interval.Minute)]
public sealed class CleanupTemporaryDataResets : IJob
{
    private readonly ITemporaryDataStore _passwordResetManager;
    private readonly ILogger<CleanupTemporaryDataResets> _logger;

    public CleanupTemporaryDataResets(ITemporaryDataStore passwordResetManager, ILogger<CleanupTemporaryDataResets> logger)
    {
        _passwordResetManager = passwordResetManager;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Cleaning up expired temp data...");
        int nRemoved = await _passwordResetManager.CleanupExpiredData();
        _logger.LogInformation("Removed {nRemoved} reset request entities", nRemoved);
    }
}