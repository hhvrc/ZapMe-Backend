using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Options;
using ZapMe.Options;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class DiscordBotService : IDiscordBotService
{
    private readonly DiscordClient _client;
    private readonly ILogger<DiscordBotService> _logger;

    public DiscordBotService(IOptions<DiscordBotOptions> options, ILoggerFactory loggerFactory, ILogger<DiscordBotService> logger)
    {
        _logger = logger;
        _client = new DiscordClient(new DiscordConfiguration
        {
            Token = options.Value.Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged,
            LoggerFactory = loggerFactory,
        });
        IsConnected = false;
    }

    public bool IsConnected { get; private set; }

    public async Task SetActivityAsync(DiscordActivity activity, UserStatus? status, DateTimeOffset? idleSince)
    {
        if (IsConnected)
        {
            await _client.UpdateStatusAsync(activity, status, idleSince);
        }
        else
        {
            await _client.ConnectAsync(activity, status, idleSince);
            IsConnected = true;
        }
    }
}
