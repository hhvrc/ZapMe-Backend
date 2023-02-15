﻿using Quartz;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ZapMe.Attributes;

namespace ZapMe.Extensions.DependencyInjection;

public static class QuartzIServiceCollectionExtensions
{
    readonly struct TimerTrigger
    {
        public Type ClassType { get; init; }
        public QuartzTimerAttribute Attribute { get; init; }
    }

    static bool IsQuartzTimerClass(Type type)
    {
        return type.IsClass
            && !type.IsAbstract
            && typeof(IJob).IsAssignableFrom(type)
            && type.GetCustomAttributes().Any(a => a is QuartzTimerAttribute);
    }

    static IEnumerable<TimerTrigger> GetAllQuartzTimers()
    {

        return Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(IsQuartzTimerClass)
            .Select(timerType =>
                new TimerTrigger
                {
                    ClassType = timerType,
                    Attribute = timerType.GetCustomAttribute<QuartzTimerAttribute>()!
                });
    }

    static void ConfigureQuartz(IServiceCollectionQuartzConfigurator conf)
    {
        conf.UseMicrosoftDependencyInjectionJobFactory();

        foreach (TimerTrigger trigger in GetAllQuartzTimers())
        {
            QuartzTimerAttribute attribute = trigger.Attribute;

            JobKey jobKey = new JobKey(attribute.TriggerName);

            conf.AddJob(
                trigger.ClassType,
                jobKey,
                opts => opts.WithIdentity(jobKey)
                );

            conf.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity(attribute.TriggerName + "-trigger")
                .WithCronSchedule(attribute.CronExpression)
                );
        }
    }

    static void ConfigureQuartzServer(QuartzHostedServiceOptions opt)
    {
        opt.WaitForJobsToComplete = true;
    }

    public static void ZMAddQuartz([NotNull] this IServiceCollection services)
    {
        services.AddQuartz(ConfigureQuartz);
        services.AddQuartzServer(ConfigureQuartzServer);
    }
}