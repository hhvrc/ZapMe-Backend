using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using ZapMe.Services.Interfaces;
using ZapMe.Websocket;

namespace ZapMe.Services;

public sealed class WebSocketClientHub : IWebSocketClientHub
{
    private readonly ILogger<WebSocketClientHub> _logger;
    private readonly ConcurrentDictionary<Guid, WebSocketClient> _clients = new();
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, WebSocketClient>> _users = new();

    public WebSocketClientHub(ILogger<WebSocketClientHub> logger)
    {
        _logger = logger;
    }

    public uint OnlineCount => (uint)_users.Count;

    public async Task<bool> RegisterClientAsync(WebSocketClient client, CancellationToken cancellationToken)
    {
        Guid userId = client.UserId;

        // Get user clients
        if (!_users.TryGetValue(userId, out var userClients) || userClients == null)
        {
            userClients = new ConcurrentDictionary<Guid, WebSocketClient>();
            if (!_users.TryAdd(userId, userClients))
            {
                if (!_users.TryGetValue(userId, out userClients) || userClients == null)
                {
                    await client.CloseAsync(WebSocketCloseStatus.InternalServerError, "Failed to register user", cancellationToken);
                    return false;
                }
            }

            // Mark user as online
        }

        Guid sessionId = client.SessionId;

        // Add client
        if (!userClients.TryAdd(sessionId, client))
        {
            if (userClients.TryGetValue(sessionId, out var existingClient) && existingClient is not null)
            {
                await existingClient.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Disconnected by another client", cancellationToken);
            }
            if (!userClients.TryAdd(sessionId, client))
            {
                await client.CloseAsync(WebSocketCloseStatus.InternalServerError, "Failed to register client", cancellationToken);
                return false;
            }
        }

        try
        {
            // Add to redis

            // Publish event
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register client");
            await RemoveClientAsync(sessionId, cancellationToken);
            await client.CloseAsync(WebSocketCloseStatus.InternalServerError, "Failed to register client", cancellationToken);
            return false;
        }

        return true;
    }

    public async Task RemoveClientAsync(Guid clientSessionId, CancellationToken cancellationToken)
    {
        if (_clients.TryRemove(clientSessionId, out var client) && client is not null)
        {
            Guid userId = client.UserId;

            if (_users.TryGetValue(userId, out var userClients) && userClients is not null)
            {
                userClients.TryRemove(clientSessionId, out _);
                if (userClients.IsEmpty)
                {
                    _users.TryRemove(userId, out _);

                    // Mark user as offline
                }

                // Remove from redis

                // Publish event

                await Task.CompletedTask;
            }
        }
    }

    public async Task DisconnectUserClientsAsync(Guid userId, WebSocketCloseStatus closeStatus, string reason, CancellationToken cancellationToken)
    {
        if (_users.TryRemove(userId, out var userClients) && userClients is not null)
        {
            foreach (var client in userClients.Values.ToArray())
            {
                _clients.TryRemove(client.SessionId, out _);
                userClients.TryRemove(client.SessionId, out _);
                await client.CloseAsync(closeStatus, reason, cancellationToken);
            }

            // Mark user as offline

            // Remove from redis

            // Publish event
        }
    }

    public async Task DisconnectAllClientsAsync(WebSocketCloseStatus closeStatus, string reason, CancellationToken cancellationToken)
    {
        foreach (var user in _users.ToArray())
        {
            _users.TryRemove(user.Key, out _);
            foreach (var client in user.Value.ToArray())
            {
                _clients.TryRemove(client.Key, out _);
                user.Value.TryRemove(client.Key, out _);
                await client.Value.CloseAsync(closeStatus, reason, cancellationToken);
            }

            // Mark user as offline

            // Remove from redis

            // Publish event
        }
    }

    public async Task RunActionOnClientAsync(Guid clientSessionId, Func<WebSocketClient, Task> action, CancellationToken cancellationToken)
    {
        if (_clients.TryGetValue(clientSessionId, out var client) && client is not null)
        {
            await action(client);
        }
    }

    public async Task RunActionOnUserClientsAsync(Guid userId, Func<WebSocketClient, Task> action, CancellationToken cancellationToken)
    {
        if (_users.TryGetValue(userId, out var userClients) && userClients is not null)
        {
            foreach (var client in userClients.Values.ToArray())
            {
                await action(client);
            }
        }
    }

    public async Task RunActionOnAllClientsAsync(Func<WebSocketClient, Task> action, CancellationToken cancellationToken)
    {
        foreach (var user in _users.ToArray())
        {
            foreach (var client in user.Value.Values.ToArray())
            {
                await action(client);
            }
        }
    }
}