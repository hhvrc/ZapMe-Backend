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
        DiscordActivity activity = new DiscordActivity($"with {_webSocketInstanceManager.OnlineCount} users", ActivityType.Playing);
        await _discordBotService.SetActivityAsync(activity);
    }
}