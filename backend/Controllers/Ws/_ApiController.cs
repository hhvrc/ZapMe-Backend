using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Ws;

/// <summary>
/// 
/// </summary>
[Consumes(Application.Json)]
[Produces(Application.Json)]
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
