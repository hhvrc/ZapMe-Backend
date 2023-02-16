namespace ZapMe.Attributes;

public static class QuartzTimer
{
    /// <summary>
    /// Predefined time intervals
    /// </summary>
    public enum Predefined
    {
        /// <summary>
        /// January 1st at 00:00
        /// </summary>
        Yearly,

        /// <summary>
        /// 1st day of every month at 00:00
        /// </summary>
        Monthly,

        /// <summary>
        /// Monday, every week at 00:00
        /// </summary>
        Weekly,

        /// <summary>
        /// Everyday at 00:00
        /// </summary>
        Daily,

        /// <summary>
        /// Every hour
        /// </summary>
        Hourly
    }

    /// <summary>
    /// Enums used for defining intervals for cron expressions
    /// </summary>
    public enum Interval
    {
        /// <summary>
        /// Range: 1-11
        /// </summary>
        Month,

        /// <summary>
        /// Range: 1-30
        /// </summary>
        Day,

        /// <summary>
        /// Range: 1-23
        /// </summary>
        Hour,

        /// <summary>
        /// Range: 0-59
        /// </summary>
        Minute
    }

    /// <summary>
    /// 
    /// </summary>
    public enum Weekly
    {
        Monday,
        Tuesday,
        Wedensday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public static string ToCronString(this Weekly weekly)
    {
        return weekly.ToString()[..3].ToUpperInvariant();
    }
}

/// <summary>
/// Helper attribute for registering a Cron job to Quartz
/// This attribute must be put on a class that inherits from IJob, and will automatically get registered if ZMAddQuartz() has been ran in the service builder
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class QuartzTimerAttribute : Attribute
{
    public string TriggerName { get; }
    public string CronExpression { get; }

    public QuartzTimerAttribute(string triggerName, string cronExpression)
    {
        TriggerName = triggerName;
        CronExpression = cronExpression;
    }
    public QuartzTimerAttribute(string triggerName, QuartzTimer.Predefined predefined)
    {
        TriggerName = triggerName;
        CronExpression = predefined switch
        {
            QuartzTimer.Predefined.Yearly => "0 0 0 1 1 ?",
            QuartzTimer.Predefined.Monthly => "0 0 0 1 1/1 ?",
            QuartzTimer.Predefined.Weekly => "0 0 0 ? * MON",
            QuartzTimer.Predefined.Daily => "0 0 0 1/1 * ?",
            QuartzTimer.Predefined.Hourly => "0 0 0/1 1/1 * ?",
            _ => throw new ArgumentOutOfRangeException(nameof(predefined), (int)predefined, null)
        };
    }
    public QuartzTimerAttribute(string triggerName, int interval, QuartzTimer.Interval timeUnit)
    {
        TriggerName = triggerName;
        CronExpression = timeUnit switch
        {
            QuartzTimer.Interval.Month => $"0 0 0 1 1/{interval} ?",
            QuartzTimer.Interval.Day => $"0 0 0 1/{interval} * ?",
            QuartzTimer.Interval.Hour => $"0 0 0/{interval} * * ?",
            QuartzTimer.Interval.Minute => $"0 0/{interval} * * * ?",
            _ => throw new ArgumentOutOfRangeException(nameof(timeUnit), (int)timeUnit, null)
        };
    }
    public QuartzTimerAttribute(string triggerName, int hour, int minute, QuartzTimer.Weekly weekday)
    {
        TriggerName = triggerName;
        CronExpression = $"0 {minute} {hour} ? * {weekday.ToCronString()}";
    }
    public QuartzTimerAttribute(string triggerName, int hour, int minute, params QuartzTimer.Weekly[] weekdays)
    {
        TriggerName = triggerName;
        CronExpression = $"0 {minute} {hour} ? * {String.Join(',', weekdays.Select(static wd => wd.ToCronString()))}";
    }
    public QuartzTimerAttribute(string triggerName, QuartzTimer.Weekly weekday) : this(triggerName, 0, 0, weekday)
    {
    }
    public QuartzTimerAttribute(string triggerName, params QuartzTimer.Weekly[] weekdays) : this(triggerName, 0, 0, weekdays)
    {
    }
}
