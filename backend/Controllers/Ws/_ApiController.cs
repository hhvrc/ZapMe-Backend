using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Ws;

/// <summary>
/// 
/// </summary>
[ApiController, Authorize, Route("ws")]
public sealed partial class WebSocketController : Controller
{
    private readonly IWebSocketInstanceManager _webSocketInstanceManager;
    private readonly ILogger<WebSocketController> _logger;

    public WebSocketController(IWebSocketInstanceManager webSocketInstanceManager, ILogger<WebSocketController> logger)
    {
        _webSocketInstanceManager = webSocketInstanceManager;
        _logger = logger;
    }
}
