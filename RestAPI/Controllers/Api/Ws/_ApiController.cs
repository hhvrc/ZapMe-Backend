using Microsoft.AspNetCore.Mvc;
using ZapMe.DTOs;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.Ws;

/// <summary>
/// 
/// </summary>
[Consumes(Application.Json)]
[Produces(Application.Json)]
[ProducesErrorResponseType(typeof(ErrorDetails))]
[ApiController, Route("api/ws")]
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
