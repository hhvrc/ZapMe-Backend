using fbs.server;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using ZapMe.BusinessLogic.CQRS.Events;
using ZapMe.Constants;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.Enums.Errors;
using ZapMe.Services.Interfaces;
using static ZapMe.Websocket.WebSocketHub;

namespace ZapMe.Websocket;

public sealed class WebSocketHandler : IWebSocketHandler
{
    private readonly IMediator _mediator;
    private readonly IJwtAuthenticationManager _authenticationManager;
    private readonly DatabaseContext _dbContext;
    private readonly ILogger<WebSocketHandler> _logger;

    public WebSocketHandler(IMediator mediator, IJwtAuthenticationManager authenticationManager, DatabaseContext dbContext, ILogger<WebSocketHandler> logger)
    {
        _mediator = mediator;
        _authenticationManager = authenticationManager;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task RunAsync(Func<string?, Task<WebSocket>> webSocketAcceptFunc, IList<string> requestedSubProtocols, CancellationToken cancellationToken)
    {
        if (!requestedSubProtocols.Contains("binary") || !requestedSubProtocols.Contains("fbs"))
        {
            _logger.LogError("Failed to authenticate websocket connection, client did not request binary and fbs protocols");
            return;
        }

        // Get protocols of interest
        string clientType = requestedSubProtocols.Single(p => p.StartsWith("client_"));

        // Accept websocket connection
        using WebSocket webSocket = await webSocketAcceptFunc(clientType);

        // Run client specific logic
        switch (clientType)
        {
            case "client_user":
                await RunUserAsync(webSocket, cancellationToken);
                break;
            case "client_device":
                await RunDeviceAsync(webSocket, cancellationToken);
                break;
            default:
                _logger.LogError("Failed to authenticate websocket connection, client requested unknown protocol");
                break;
        }
    }

    private async Task RunUserAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        // Receive JWT from client
        string? token = await webSocket.ReceiveStringAsync(cancellationToken);
        if (token is null)
        {
            _logger.LogError("Failed to authenticate websocket connection, received invalid message from client");
            return;
        }

        // Validate JWT
        var authenticationResult = await _authenticationManager.AuthenticateJwtTokenAsync(token, cancellationToken);
        if (authenticationResult.TryPickT1(out JwtAuthenticationError authenticationError, out SessionEntity session))
        {
            _logger.LogError("Failed to authenticate websocket connection, provided JWT was invalid");
            return;
        }

        // Success, create websocket instance
        using UserWebSocket instance = new UserWebSocket(session.UserId, session.Id, webSocket);

        // Send hello message to inform client that everything is A-OK
        await instance.SendPayloadAsync(new ServerPayload(new ServerReady
        {
            HeartbeatIntervalMs = 10 * 1000, // 10 seconds TODO: make this configurable
            RatelimitBytesPerSec = WebsocketConstants.ClientRateLimitBytesPerSecond,
            RatelimitBytesPerMin = WebsocketConstants.ClientRateLimitBytesPerMinute,
            RatelimitMessagesPerSec = WebsocketConstants.ClientRateLimitMessagesPerSecond,
            RatelimitMessagesPerMin = WebsocketConstants.ClientRateLimitMessagesPerMinute,
        }), cancellationToken);

        // Register instance globally, the hub will have the ability to kill this connection
        RegistrationResult result = await RegisterClientAsync(instance, cancellationToken);
        if (result is not (RegistrationResult.Ok or RegistrationResult.OkUserOnline))
        {
            await instance.CloseAsync(WebSocketCloseStatus.InternalServerError, "Failed to register websocket connection", cancellationToken);
            _logger.LogError("Failed to register websocket connection, the hub was unable to register the client");
            return;
        }

        // Finally, run set the user as online and run the websocket, with some cleanup logic at the end
        try
        {
            if (result == RegistrationResult.OkUserOnline)
            {
                await _mediator.Publish(new UserOnlineEvent(session.UserId), cancellationToken);
            }

            await instance.RunWebSocketAsync(cancellationToken);
        }
        finally
        {
            // Remove instance globally
            RemoveClientResult removeResult = RemoveClient(instance.SessionId, cancellationToken);

            if (removeResult == RemoveClientResult.OkUserOffline)
            {
                CancellationToken ct = new CancellationTokenSource(1000).Token;
                await _mediator.Publish(new UserOfflineEvent(session.UserId), ct);
            }
        }
    }

    private async Task RunDeviceAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        // Receive AccessToken from client
        string? accessToken = await webSocket.ReceiveStringAsync(cancellationToken);
        if (accessToken is null)
        {
            _logger.LogError("Failed to authenticate websocket connection, received invalid message from client");
            return;
        }

        // Validate AccessToken
        var authenticationResult = await _dbContext.Devices
            .Where(d => d.AccessToken == accessToken)
            .Select(d => new { d.Id, d.OwnerId })
            .FirstOrDefaultAsync(cancellationToken);
        if (authenticationResult is null)
        {
            _logger.LogError("Failed to authenticate websocket connection, provided AccessToken was invalid");
            return;
        }

        // Success, create websocket instance
        using DeviceWebSocket instance = new DeviceWebSocket(authenticationResult.OwnerId, authenticationResult.Id, webSocket);

        await instance.RunWebSocketAsync(cancellationToken);
    }
}
