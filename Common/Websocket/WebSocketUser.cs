using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ZapMe.Websocket;

public sealed class WebSocketUser
{
    private readonly ConcurrentDictionary<Guid, WebSocketClient> _clients = new();
    private readonly object _deadLock = new(); // This is a horrible name LOL
    private bool _dead = false;

    public Guid UserId { get; }
    public bool IsOnline => !_clients.IsEmpty;

    public WebSocketUser(Guid userId)
    {
        UserId = userId;
    }

    /// <summary>
    /// Adds a client to the user, will disconnect any existing client with the same session ID
    /// </summary>
    public async Task<bool> TryAddClientAsync(WebSocketClient client, CancellationToken cancellationToken = default)
    {
        Guid sessionId = client.SessionId;
        Task? closeTask = null;

        // Try-finally to ensure the close task is awaited
        try
        {
            // Lock to ensure only one client is added at a time, this only applies to adding clients
            // If the user is being disconnected, we don't want clients added during the disconnect
            lock (_deadLock)
            {
                // If the user is being removed, don't add the client
                if (_dead)
                {
                    return false;
                }

                // Try to add the client
                if (_clients.TryAdd(sessionId, client))
                {
                    return true;
                }

                // Add failed, close the existing client
                if (_clients.TryRemove(sessionId, out WebSocketClient? existingClient) && existingClient is not null)
                {
                    closeTask = existingClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnected by another client", cancellationToken);
                }

                // Now try to add again
                if (_clients.TryAdd(sessionId, client))
                {
                    return true;
                }

                // Add failed again, give up
                return false;
            }
        }
        finally
        {
            // If there was a existing client, await the close task
            if (closeTask is not null)
            {
                await closeTask;
            }
        }
    }

    public bool TryRemoveClient(Guid clientSessionId, out WebSocketClient? client)
    {
        return _clients.TryRemove(clientSessionId, out client);
    }

    public async Task<bool> TryDisconnectClientAsync(Guid clientSessionId, WebSocketCloseStatus closeStatus, string closeReason, CancellationToken cancellationToken = default)
    {
        if (_clients.TryRemove(clientSessionId, out WebSocketClient? client) && client is not null)
        {
            await client.CloseAsync(closeStatus, closeReason, cancellationToken);
            return true;
        }

        return false;
    }

    public Task DisconnectAllAsync(WebSocketCloseStatus closeStatus, string closeReason, CancellationToken cancellationToken = default)
    {
        List<Task> tasks = new();

        lock (_deadLock)
        {
            _dead = true;
            foreach (WebSocketClient client in _clients.Values)
            {
                tasks.Add(client.CloseAsync(closeStatus, closeReason, cancellationToken));
            }
        }

        return Task.WhenAll(tasks);
    }

    public Task RunActionOnClientAsync(Guid clientSessionId, Func<WebSocketClient, Task> action)
    {
        if (_clients.TryGetValue(clientSessionId, out WebSocketClient? client) && client is not null)
        {
            return action(client);
        }

        return Task.CompletedTask;
    }

    public async Task RunActionOnAllClientsAsync(Func<WebSocketClient, Task> action)
    {
        foreach (WebSocketClient client in _clients.Values)
        {
            await action(client);
        }
    }
}
