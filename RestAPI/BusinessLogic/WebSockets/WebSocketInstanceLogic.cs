using System.Net.WebSockets;
using ZapMe.DTOs;
using ZapMe.Enums.Errors;
using ZapMe.Helpers;
using ZapMe.Mappers;
using ZapMe.Services.Interfaces;
using ZapMe.Websocket;

namespace ZapMe.BusinessLogic.WebSockets;

public static class WebSocketInstanceLogic
{
    private static async Task<ErrorDetails?> RunWebSocketAsync(HttpContext httpContext, WebSocket websocket, CancellationToken cancellationToken)
    {
        // Services
        IWebSocketInstanceManager wsInstanceManager = httpContext.RequestServices.GetRequiredService<IWebSocketInstanceManager>();
        IJwtAuthenticationManager jwtAuthenticationManager = httpContext.RequestServices.GetRequiredService<IJwtAuthenticationManager>();
        ILogger<WebSocketInstance> wsLogger = httpContext.RequestServices.GetRequiredService<ILogger<WebSocketInstance>>();

        // Create the connection instance
        var result = await WebSocketInstance.CreateAsync(websocket, jwtAuthenticationManager, wsLogger, cancellationToken);
        if (result.TryPickT1(out CreateWebSocketError createError, out WebSocketInstance instance))
        {
            return CreateWebSocketErrorMapper.MapToErrorDetails(createError);
        }

        // The trace identifier is used to identify the websocket instance, it will be unique for each websocket connection
        string instanceId = httpContext.TraceIdentifier;

        // Register instance globally, the manager will have the ability to kill this connection
        if (!await wsInstanceManager.RegisterInstanceAsync(instance.UserId, instanceId, instance, cancellationToken))
        {
            return HttpErrors.InternalServerError;
        }

        try
        {
            // Start communicating with the client
            await instance.RunAsync(cancellationToken);
        }
        finally
        {
            // Remove instance globally
            await wsInstanceManager.RemoveInstanceAsync(instanceId, cancellationToken: cancellationToken);
        }

        return null;
    }

    public static async Task<ErrorDetails?> HandleWebSocketAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        WebSocketManager wsManager = httpContext.WebSockets;
        if (!wsManager.IsWebSocketRequest)
        {
            return HttpErrors.Generic(StatusCodes.Status400BadRequest, "Bad request", "This endpoint is purely just a websocket endpoint", "Try to connect with a websocket client instead");
        }

        WebSocket websocket = await wsManager.AcceptWebSocketAsync();
        try
        {
            return await RunWebSocketAsync(httpContext, websocket, cancellationToken);
        }
        finally
        {
            websocket.Dispose();
        }
    }
}
