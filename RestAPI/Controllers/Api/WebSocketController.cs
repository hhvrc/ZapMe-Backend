using Microsoft.AspNetCore.Mvc;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Websocket;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api;

[ApiController]
public sealed class WebSocketController : ControllerBase
{
    /// <summary>
    /// Websocket endpoint for realtime communication, authenticate using JWT, and serialize messages using FlatBuffers
    /// </summary>
    /// <response code="400">This endpoint is purely just a websocket endpoint</response>
    [HttpGet("api/ws", Name = "WebsocketEndpoint")]
    [Produces(Application.Json)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    public async Task EntryPointAsync([FromServices] IWebSocketHandler webSocketHandler, CancellationToken cancellationToken)
    {
        WebSocketManager wsManager = HttpContext.WebSockets;

        if (!wsManager.IsWebSocketRequest)
        {
            ErrorDetails error = HttpErrors.Generic(StatusCodes.Status400BadRequest, "Bad request", "This endpoint is purely just a websocket endpoint", "Try to connect with a websocket client instead");
            await error.Write(Response);
            return;
        }

        await webSocketHandler.RunAsync(wsManager.AcceptWebSocketAsync, wsManager.WebSocketRequestedProtocols, cancellationToken);
    }
}
