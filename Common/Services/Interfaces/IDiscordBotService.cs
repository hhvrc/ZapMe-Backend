using DSharpPlus.Entities;

namespace ZapMe.Services.Interfaces;
public interface IDiscordBotService
{
    bool IsConnected { get; }

    Task SetActivityAsync(DiscordActivity activity, UserStatus? status = null, DateTimeOffset? idleSince = null);
}