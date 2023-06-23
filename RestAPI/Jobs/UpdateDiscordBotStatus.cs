using DSharpPlus.Entities;
using Quartz;
using ZapMe.Attributes;
using ZapMe.Services.Interfaces;

namespace ZapMe.Jobs;

/// <summary>
/// Updates the Discord bot's status
/// </summary>
[QuartzTimer("UpdateDiscordBotStatus", 1, QuartzTimer.Interval.Minute)]
public sealed class UpdateDiscordBotStatus : IJob
{
    private readonly IDiscordBotService _discordBotService;
    private readonly IWebSocketInstanceManager _webSocketInstanceManager;
    private readonly ILogger<UpdateDiscordBotStatus> _logger;

    public UpdateDiscordBotStatus(IDiscordBotService discordBotService, IWebSocketInstanceManager webSocketInstanceManager, ILogger<UpdateDiscordBotStatus> logger)
    {
        _discordBotService = discordBotService;
        _webSocketInstanceManager = webSocketInstanceManager;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        ulong onlineCount = _webSocketInstanceManager.OnlineCount;
        string activityText = onlineCount switch
        {
            0 => "with noone 😢",
            1 => "with 1 user 🥺",
            69 => "with 69 users 😏",
            420 => "with 420 users 😶",
            _ => $"with {onlineCount} users"
        };
        UserStatus userStatus = onlineCount switch
        {
            0 => UserStatus.DoNotDisturb,
            _ => UserStatus.Online
        };

        DiscordActivity activity = new DiscordActivity(activityText, ActivityType.Playing);
        await _discordBotService.SetActivityAsync(activity, userStatus);
    }
}