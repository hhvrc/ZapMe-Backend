using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using ZapMe.BusinessLogic.CQRS.Events;
using ZapMe.BusinessLogic.Serialization.Flatbuffers;
using ZapMe.Constants;
using ZapMe.Database.Models;
using ZapMe.Enums.Errors;
using ZapMe.Services.Interfaces;
using ZapMe.Websocket;
using static ZapMe.Websocket.WebSocketHub;

namespace ZapMe.BusinessLogic;

public static class WebSocketHandler
{
    public static async Task Run(WebSocket webSocket, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        // Get logger that will be passed to the websocket instance
        var logger = serviceProvider.GetRequiredService<ILogger<WebSocketClient>>();

        // Receive JWT from client
        string? token = await webSocket.ReceiveStringAsync(cancellationToken);
        if (token is null)
        {
            logger.LogError("Failed to authenticate websocket connection, received invalid message from client");
            return;
        }

        // Get JWT authentication manager
        var authenticationManager = serviceProvider.GetRequiredService<IJwtAuthenticationManager>();

        // Validate JWT
        var authenticationResult = await authenticationManager.AuthenticateJwtTokenAsync(token, cancellationToken);
        if (authenticationResult.TryPickT1(out JwtAuthenticationError authenticationError, out SessionEntity session))
        {
            logger.LogError("Failed to authenticate websocket connection, provided JWT was invalid");
            return;
        }

        // Success, create websocket instance
        var instance = new WebSocketClient(session.UserId, session.Id, webSocket, logger);

        // Send hello message to inform client that everything is A-OK
        await ServerReadySerializer.Serialize(
            heartbeatIntervalMs: 10 * 1000, // 10 seconds TODO: make this configurable
            ratelimitBytesPerSec: WebsocketConstants.ClientRateLimitBytesPerSecond,
            ratelimitBytesPerMin: WebsocketConstants.ClientRateLimitBytesPerMinute,
            ratelimitMessagesPerSec: WebsocketConstants.ClientRateLimitMessagesPerSecond,
            ratelimitMessagesPerMin: WebsocketConstants.ClientRateLimitMessagesPerMinute,
            (bytes) => instance.SendMessageAsync(bytes, cancellationToken)
            );

        // Get mediator to send userOnline/userOffline events
        IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

        // Register instance globally, the hub will have the ability to kill this connection
        var result = await RegisterClientAsync(instance, cancellationToken);
        if (result is not RegistrationResult.Ok or RegistrationResult.OkUserOnline)
        {
            logger.LogError("Failed to register websocket connection, the hub was unable to register the client");
            return;
        }

        // Finally, run set the user as online and run the websocket, with some cleanup logic at the end
        try
        {
            await mediator.Publish(new UserOnlineEvent(session.UserId), cancellationToken);

            await instance.RunWebSocketAsync(cancellationToken);
        }
        finally
        {
            await mediator.Publish(new UserOfflineEvent(session.UserId), cancellationToken);

            // Remove instance globally
            RemoveClientAsync(instance.SessionId);
        }
    }
}
