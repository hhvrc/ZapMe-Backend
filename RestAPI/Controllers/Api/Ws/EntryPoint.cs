using Microsoft.AspNetCore.Mvc;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Websocket;

namespace ZapMe.Controllers.Api.Ws;

public sealed partial class WebSocketController
{
    /// <summary>
    /// Websocket endpoint for pub/sub communication (e.g. chat, notifications, events)
    /// 
    /// Documentation:
    /// Yes
    /// </summary>
    /// <param name="token"></param>
    /// <param name="authenticationManager"></param>
    /// <param name="logger"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Connection closed</response>
    /// <response code="400">This endpoint is purely just a websocket endpoint</response>
    [HttpGet(Name = "WebSocket")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task EntryPointAsync(
        [FromQuery] string token,
        [FromServices] IJwtAuthenticationManager authenticationManager,
        [FromServices] ILogger<WebSocketInstance> logger,
        CancellationToken cancellationToken
        )
    {
        ErrorDetails errorDetails;
        WebSocketManager wsManager = HttpContext.WebSockets;

        if (!wsManager.IsWebSocketRequest)
        {
            errorDetails = HttpErrors.Generic(StatusCodes.Status400BadRequest, "Bad request", "This endpoint is purely just a websocket endpoint", "Try to connect with a websocket client instead");
            await errorDetails.Write(Response);
            return;
        }

        var authenticationResult = await authenticationManager.AuthenticateJwtTokenAsync(token, cancellationToken);
        if (authenticationResult.TryPickT1(out errorDetails, out SessionEntity session))
        {
            await errorDetails.Write(Response);
            return;
        }

        // The trace identifier is used to identify the websocket instance, it will be unique for each websocket connection
        string instanceId = HttpContext.TraceIdentifier;

        // Create the connection instance
        using WebSocketInstance? instance = await WebSocketInstance.CreateAsync(wsManager, session, logger);
        if (instance is null)
        {
            _logger.LogError("Failed to create websocket instance");

            await HttpErrors.InvalidSSOToken.Write(Response);
            return;
        }

        // Register instance globally, the manager will have the ability to kill this connection
        if (!await _webSocketInstanceManager.RegisterInstanceAsync(session.UserId, instanceId, instance, cancellationToken))
        {
            await HttpErrors.InternalServerError.Write(Response);
            return;
        }

        try
        {
            // Start communicating with the client
            await instance.RunAsync(cancellationToken);
        }
        finally
        {
            // Remove instance globally
            await _webSocketInstanceManager.RemoveInstanceAsync(instanceId, cancellationToken: cancellationToken);
        }
    }
}
