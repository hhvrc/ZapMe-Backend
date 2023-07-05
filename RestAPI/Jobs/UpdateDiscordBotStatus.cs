using DSharpPlus.Entities;
using Quartz;
using ZapMe.Attributes;
using ZapMe.Services.Interfaces;
using ZapMe.Websocket;

namespace ZapMe.Jobs;

/// <summary>
/// Updates the Discord bot's status
/// </summary>
[QuartzTimer("UpdateDiscordBotStatus", 1, QuartzTimer.Interval.Minute)]
public sealed class UpdateDiscordBotStatus : IJob
{
    private readonly IDiscordBotService _discordBotService;
    private readonly ILogger<UpdateDiscordBotStatus> _logger;

    public UpdateDiscordBotStatus(IDiscordBotService discordBotService, ILogger<UpdateDiscordBotStatus> logger)
    {
        _discordBotService = discordBotService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        int onlineCount = WebSocketHub.Users.Count;
        string activityText = onlineCount switch
        {
            0 => "with no online users",
            1 => "with 1 online user",
            _ => $"with {onlineCount} online users"
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