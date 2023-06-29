using System.Net.WebSockets;
using ZapMe.DTOs;
using ZapMe.Enums.Errors;
using ZapMe.Helpers;
using ZapMe.Mappers;
using ZapMe.Services.Interfaces;

namespace ZapMe.ApplicationLayer.WebSockets;

public static class WebSocketInstanceLayer
{
    private static async Task<ErrorDetails?> RunWebSocketAsync(HttpContext httpContext, WebSocket websocket, CancellationToken cancellationToken)
    {
        // Services
        IWebSocketInstanceManager wsInstanceManager = httpContext.RequestServices.GetRequiredService<IWebSocketInstanceManager>();
        IJwtAuthenticationManager jwtAuthenticationManager = httpContext.RequestServices.GetRequiredService<IJwtAuthenticationManager>();
        ILogger<Websocket.WebSocketInstance> wsLogger = httpContext.RequestServices.GetRequiredService<ILogger<Websocket.WebSocketInstance>>();

        // Create the connection instance
        var result = await Websocket.WebSocketInstance.CreateAsync(websocket, jwtAuthenticationManager, wsLogger, cancellationToken);
        if (result.TryPickT1(out CreateWebSocketError createError, out Websocket.WebSocketInstance instance))
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

    public static async Task HandleWebSocketAsync(HttpResponse httpResponse, HttpContext httpContext, CancellationToken cancellationToken)
    {
        WebSocketManager wsManager = httpContext.WebSockets;
        if (!wsManager.IsWebSocketRequest)
        {
            await HttpErrors.Generic(StatusCodes.Status400BadRequest, "Bad request", "This endpoint is purely just a websocket endpoint", "Try to connect with a websocket client instead").Write(httpResponse);
            return;
        }

        WebSocket websocket = await wsManager.AcceptWebSocketAsync();
        try
        {
            ErrorDetails? errorDetails = await RunWebSocketAsync(httpContext, websocket, cancellationToken);
            if (errorDetails.HasValue)
            {
                await errorDetails.Value.Write(httpResponse);
            }
        }
        finally
        {
            websocket.Dispose();
        }
    }
}
