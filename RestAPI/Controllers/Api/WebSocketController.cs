using Microsoft.AspNetCore.Mvc;
using ZapMe.BusinessLogic.WebSockets;
using ZapMe.DTOs;
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
    public async Task EntryPointAsync(CancellationToken cancellationToken)
    {
        ErrorDetails? errorDetails = await WebSocketInstanceLogic.HandleWebSocketAsync(HttpContext, cancellationToken);
        if (errorDetails.HasValue)
        {
            await errorDetails.Value.Write(Response);
        }
    }
}
