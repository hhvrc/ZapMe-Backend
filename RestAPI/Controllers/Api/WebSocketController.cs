using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using ZapMe.BusinessLogic;
using ZapMe.DTOs;
using ZapMe.Helpers;
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
        WebSocketManager wsManager = HttpContext.WebSockets;
        IServiceProvider serviceProvider = HttpContext.RequestServices;

        if (!wsManager.IsWebSocketRequest)
        {
            await HttpErrors.Generic(StatusCodes.Status400BadRequest, "Bad request", "This endpoint is purely just a websocket endpoint", "Try to connect with a websocket client instead").Write(Response);
            return;
        }

        using WebSocket websocket = await wsManager.AcceptWebSocketAsync();

        await WebSocketHandler.Run(websocket, serviceProvider, cancellationToken);
    }
}
