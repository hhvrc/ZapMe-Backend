using Quartz;
using ZapMe.Attributes;

namespace ZapMe.Jobs;

/// <summary>
/// For now, this is just a example/placeholder for the weekly job.
/// </summary>
[QuartzTimer("WeeklyJob", QuartzTimer.Predefined.Weekly)]
public class WeeklyJob : IJob
{
    private readonly ILogger<WeeklyJob> _logger;

    public WeeklyJob(ILogger<WeeklyJob> logger)
    {
        _logger = logger;
    }

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Weekly job executed!");
        return Task.CompletedTask;
    }
}