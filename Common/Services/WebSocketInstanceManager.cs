using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using ZapMe.Services.Interfaces;
using ZapMe.Websocket;

namespace ZapMe.Services;

public sealed class WebSocketInstanceManager : IWebSocketInstanceManager
{
    private readonly ILogger<WebSocketInstanceManager> _logger;
    private readonly ConcurrentDictionary<Guid, WebSocketClient> _clients = new();
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, WebSocketClient>> _users = new();

    public WebSocketInstanceManager(ILogger<WebSocketInstanceManager> logger)
    {
        _logger = logger;
    }

    public ulong OnlineCount => (ulong)_users.Count;

    private Task RemoveClientBySessionId(Guid sessionId)
    {
        if (_clients.TryRemove(sessionId, out var client) && client is not null)
        {
            Guid userId = client.UserId;

            if (_users.TryGetValue(userId, out var userClients) && userClients is not null)
            {
                userClients.TryRemove(sessionId, out _);
                if (userClients.Count == 0)
                {
                    _users.TryRemove(userId, out _);

                    // Mark user as offline
                }

                // Remove from redis

                // Publish event

                return Task.CompletedTask;
            }
        }
    }

    public async Task<bool> RegisterClientAsync(WebSocketClient client, CancellationToken cancellationToken)
    {
        Guid userId = client.UserId;

        // Get user clients
        if (_users.TryGetValue(userId, out var userClients) || userClients == null)
        {
            userClients = new ConcurrentDictionary<Guid, WebSocketClient>();
            if (!_users.TryAdd(userId, userClients))
            {
                await client.CloseAsync(WebSocketCloseStatus.InternalServerError, "Failed to register user", cancellationToken);
                return false;
            }

            // Mark user as online
        }

        Guid sessionId = client.SessionId;

        // Add client
        if (!userClients.TryAdd(sessionId, client))
        {
            await client.CloseAsync(WebSocketCloseStatus.PolicyViolation, "You are already connected on this authentication session", cancellationToken);
            return false;
        }

        try
        {
            // Add to redis

            // Publish event
        }
        catch
        {
            await RemoveClientBySessionId(sessionId);
            await client.CloseAsync(WebSocketCloseStatus.InternalServerError, "Failed to register client", cancellationToken);
            throw;
        }

        return true;
    }

    public Task RemoveClientAsync(Guid clientSessionId, string reason, CancellationToken cancellationToken)
    {
        return RemoveClientBySessionId(clientSessionId);
    }

    public async Task DisconnectAllClientsAsync(Guid userId, string reason, CancellationToken cancellationToken)
    {
        foreach (var instance in _users.Where(x => x.Value.UserId == userId).ToArray()) // ToArray is very important here to avoid collection modified exception
        {
            await RemoveInstanceAsync(instance.Key, reason, cancellationToken);
        }
    }

    public async Task RunActionOnInstanceAsync(Guid userId, Func<WebSocketClient, Task> action, CancellationToken cancellationToken)
    {
        if (_users.TryGetValue(instanceId, out var instance) && instance is not null)
        {
            await action(instance);
        }
    }

    public Task DisconnectEveryoneAsync(string reason = "Forcefully removed", CancellationToken cancellationToken = default) => throw new NotImplementedException();
}