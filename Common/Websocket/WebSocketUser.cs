using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ZapMe.Websocket;

public sealed class WebSocketUser
{
    private readonly ConcurrentDictionary<Guid, WebSocketClient> _clients = new();

    private const int _STATE_ALIVE = 0;
    private const int _STATE_ACCEPTING_CONNECTION = 1;
    private const int _STATE_DISCONNECTING = 2;
    private const int _STATE_DEAD = 3;
    private int _state = _STATE_ALIVE;

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

        // If the user is being removed, don't add the client
        int state = Interlocked.CompareExchange(ref _state, _STATE_ACCEPTING_CONNECTION, _STATE_ALIVE);
        if (state != _STATE_ALIVE)
        {
            return false;
        }

        // Try-finally to ensure the close task is awaited, and the state is set back to alive
        try
        {
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
        finally
        {
            // Set the state back to alive if it still is _STATE_ACCEPTING_CONNECTION
            Interlocked.CompareExchange(ref _state, _STATE_ALIVE, _STATE_ACCEPTING_CONNECTION);

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

    public async Task DisconnectAllAsync(WebSocketCloseStatus closeStatus, string closeReason, CancellationToken cancellationToken = default)
    {
        if (_state is _STATE_DISCONNECTING or _STATE_DEAD)
        {
            return;
        }

        // TODO: review this concurrency logic
        int retries = 0;
        while (Interlocked.CompareExchange(ref _state, _STATE_DISCONNECTING, _STATE_ALIVE) != _STATE_ALIVE && retries++ < 10)
        {
            await Task.Delay(10, cancellationToken);
        }
        if (_state == _STATE_ALIVE)
        {
            // We tried to wait for the state to be _STATE_ALIVE, but it never happened, force it to _STATE_DISCONNECTING
            _state = _STATE_DISCONNECTING;
        }

        List<Task> tasks = new();

        foreach (WebSocketClient client in _clients.Values)
        {
            tasks.Add(client.CloseAsync(closeStatus, closeReason, cancellationToken));
        }

        await Task.WhenAll(tasks);

        _state = _STATE_DEAD;
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
