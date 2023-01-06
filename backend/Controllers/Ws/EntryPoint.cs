using Microsoft.AspNetCore.Mvc;
using ZapMe.Authentication;

namespace ZapMe.Controllers.Ws;

public sealed partial class WebSocketController
{
    /// <summary>
    /// Websocket endpoint for pub/sub communication (e.g. chat, notifications, events)
    /// 
    /// Documentation:
    /// Yes
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Connection closed</response>
    /// <response code="400">This endpoint is purely just a websocket endpoint</response>
    [HttpGet]
    public async Task<IActionResult> EntryPointAsync(ILogger<WebSocketInstance> logger, CancellationToken cancellationToken)
    {
        WebSocketManager wsManager = HttpContext.WebSockets;

        if (wsManager.IsWebSocketRequest)
        {
            // Get the authenticated identity
            ZapMeIdentity identity = (User as ZapMePrincipal)!.Identity;

            // The trace identifier is used to identify the websocket instance, it will be unique for each websocket connection
            string instanceId = HttpContext.TraceIdentifier;

            // Create the connection instance
            using WebSocketInstance? instance = await WebSocketInstance.CreateAsync(wsManager, User, logger);
            if (instance == null)
            {
                _logger.LogError("Failed to create websocket instance");

                return this.Error_InternalServerError();
            }

            // Register instance globally, the manager will have the ability to kill this connection
            if (!await _webSocketInstanceManager.RegisterInstanceAsync(identity.AccountId, instanceId, instance, cancellationToken))
            {
                return this.Error_InternalServerError();
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
        else
        {
            return this.Error(StatusCodes.Status400BadRequest, "Bad request", "This endpoint is purely just a websocket endpoint", "Try to connect with a websocket client instead");
        }

        return Ok();
    }
}
