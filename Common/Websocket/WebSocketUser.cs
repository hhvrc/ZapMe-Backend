using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ZapMe.Websocket;

public sealed class WebSocketUser
{
    private readonly ConcurrentDictionary<Guid, UserWebSocket> _clients = new();
    private readonly ConcurrentDictionary<Guid, DeviceWebSocket> _devices = new();

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
    public async Task<bool> TryAddClientAsync(UserWebSocket client, CancellationToken cancellationToken = default)
    {
        Guid sessionId = client.SessionId;

        try
        {
            // Try to set the state to accepting connection, if it is already accepting a connection or if it is disconnecting or dead, return false
            if (Interlocked.CompareExchange(ref _state, _STATE_ACCEPTING_CONNECTION, _STATE_ALIVE) != _STATE_ALIVE)
            {
                return false;
            }

            // Try to add the client
            if (_clients.TryAdd(sessionId, client))
            {
                return true;
            }

            // Add failed, close the existing client
            if (_clients.TryRemove(sessionId, out UserWebSocket? existingClient) && existingClient is not null)
            {
                await existingClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnected by another client", cancellationToken);
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
            // Set the state back to alive if it was untouched during the acceptance process
            Interlocked.CompareExchange(ref _state, _STATE_ALIVE, _STATE_ACCEPTING_CONNECTION);
        }
    }

    public bool TryRemoveClient(Guid clientSessionId, out UserWebSocket? client)
    {
        return _clients.TryRemove(clientSessionId, out client);
    }

    public async Task<bool> TryDisconnectClientAsync(Guid clientSessionId, WebSocketCloseStatus closeStatus, string closeReason, CancellationToken cancellationToken = default)
    {
        if (_clients.TryRemove(clientSessionId, out UserWebSocket? client) && client is not null)
        {
            await client.CloseAsync(closeStatus, closeReason, cancellationToken);
            return true;
        }

        return false;
    }

    public async Task<bool> DisconnectAllAsync(WebSocketCloseStatus closeStatus, string closeReason, CancellationToken cancellationToken = default)
    {
        // Try to set the state to disconnecting if it is alive, if it is already disconnecting or dead, return true
        // This loop will try to set the state to disconnecting for 200ms, if it fails, it will return false
        int value;
        int retries = 0;
        do
        {
            value = Interlocked.CompareExchange(ref _state, _STATE_DISCONNECTING, _STATE_ALIVE);
            if (value is _STATE_DISCONNECTING or _STATE_DEAD)
            {
                return true;
            }

            if (value == _STATE_ALIVE)
            {
                break;
            }

            await Task.Delay(10, cancellationToken);
        }
        while (++retries <= 20);
        if (value != _STATE_DISCONNECTING)
        {
            return false;
        }

        List<Task> tasks = new();

        foreach (UserWebSocket client in _clients.Values)
        {
            tasks.Add(client.CloseAsync(closeStatus, closeReason, cancellationToken));
        }

        await Task.WhenAll(tasks);

        _state = _STATE_DEAD;

        return true;
    }

    public Task RunActionOnClientAsync(Guid clientSessionId, Func<UserWebSocket, Task> action)
    {
        if (_clients.TryGetValue(clientSessionId, out UserWebSocket? client) && client is not null)
        {
            return action(client);
        }

        return Task.CompletedTask;
    }

    public async Task RunActionOnAllClientsAsync(Func<UserWebSocket, ValueTask> action)
    {
        foreach (UserWebSocket client in _clients.Values)
        {
            await action(client);
        }
    }
}
