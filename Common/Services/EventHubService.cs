using Microsoft.Extensions.Logging;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class EventHubService
{
    private readonly IWebSocketInstanceManager _webSocketManager;
    private readonly ILogger<EventHubService> _logger;

    public EventHubService(IWebSocketInstanceManager websocketManager, ILogger<EventHubService> logger)
    {
        _webSocketManager = websocketManager;
        _logger = logger;
    }
}