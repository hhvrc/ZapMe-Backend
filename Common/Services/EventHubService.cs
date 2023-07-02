namespace ZapMe.Services;

public sealed class EventHubService
{
    public EventHubService(IHubContext<NotificationHub> hubContext, ILogger<EventHubService> logger)
    {
        HubContext = hubContext;
        Logger = logger;
    }

    /// <summary>
    /// Sends an event to a specific user.
    /// </summary>
    public Task SendUserEventAsync(Guid userId, string message, CancellationToken cancellationToken = default)
    {

    }

    /// <summary>
    /// Sends an event to all friends of a specific user.
    /// </summary>
    public Task SendUserFriendsEventAsync(Guid userId, string message, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}