using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Ws;

/// <summary>
/// 
/// </summary>
[ProducesErrorResponseType(typeof(ErrorDetails))]
[ApiController, Authorize, Route("ws")]
public sealed partial class WebSocketController : ControllerBase
{
    private readonly IWebSocketInstanceManager _webSocketInstanceManager;
    private readonly ILogger<WebSocketController> _logger;

    public WebSocketController(IWebSocketInstanceManager webSocketInstanceManager, ILogger<WebSocketController> logger)
    {
        _webSocketInstanceManager = webSocketInstanceManager;
        _logger = logger;
    }
}
