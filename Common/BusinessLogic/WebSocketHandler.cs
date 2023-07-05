using fbs.server;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using ZapMe.BusinessLogic.CQRS.Events;
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
        using var instance = new WebSocketClient(session.UserId, session.Id, webSocket);

        // Send hello message to inform client that everything is A-OK

        await instance.SendPayloadAsync(new ServerPayload(new ServerReady
        {
            HeartbeatIntervalMs = 10 * 1000, // 10 seconds TODO: make this configurable
            RatelimitBytesPerSec = WebsocketConstants.ClientRateLimitBytesPerSecond,
            RatelimitBytesPerMin = WebsocketConstants.ClientRateLimitBytesPerMinute,
            RatelimitMessagesPerSec = WebsocketConstants.ClientRateLimitMessagesPerSecond,
            RatelimitMessagesPerMin = WebsocketConstants.ClientRateLimitMessagesPerMinute,
        }), cancellationToken);

        // Get mediator to send userOnline/userOffline events
        IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

        // Register instance globally, the hub will have the ability to kill this connection
        var result = await RegisterClientAsync(instance, cancellationToken);
        if (result is not (RegistrationResult.Ok or RegistrationResult.OkUserOnline))
        {
            await instance.CloseAsync(WebSocketCloseStatus.InternalServerError, "Failed to register websocket connection", cancellationToken);
            logger.LogError("Failed to register websocket connection, the hub was unable to register the client");
            return;
        }

        // Finally, run set the user as online and run the websocket, with some cleanup logic at the end
        try
        {
            if (result == RegistrationResult.OkUserOnline)
            {
                await mediator.Publish(new UserOnlineEvent(session.UserId), cancellationToken);
            }

            await instance.RunWebSocketAsync(cancellationToken);
        }
        finally
        {
            // Remove instance globally
            var removeResult = RemoveClient(instance.SessionId);

            if (removeResult == RemoveClientResult.OkUserOffline)
            {
                CancellationToken ct = new CancellationTokenSource(1000).Token;
                await mediator.Publish(new UserOfflineEvent(session.UserId), ct);
            }
        }
    }
}
