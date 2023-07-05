using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ZapMe.Websocket;

public static class WebSocketHub
{
    public const WebSocketCloseStatus DefaultCloseStatus = WebSocketCloseStatus.NormalClosure;
    public const string DefaultCloseReason = "Forcefully removed";

    public static ConcurrentDictionary<Guid, WebSocketClient> Clients { get; } = new();
    public static ConcurrentDictionary<Guid, WebSocketUser> Users { get; } = new();

    public enum RegistrationResult
    {
        Ok,
        OkUserOnline,
        UserRegistrationFailed,
        ClientRegistrationFailed
    }
    public static async Task<RegistrationResult> RegisterClientAsync(WebSocketClient client, CancellationToken cancellationToken = default)
    {
        Guid clientId = client.SessionId;
        Guid userId = client.UserId;
        bool userAdded = false;

        // Set clients in global list
        Clients[clientId] = client;

        // Get user clients
        if (!Users.TryGetValue(userId, out WebSocketUser? user) || user == null)
        {
            user = new WebSocketUser(userId);
            if (!Users.TryAdd(userId, user))
            {
                if (!Users.TryGetValue(userId, out user) || user == null)
                {
                    RemoveClient(clientId);
                    return RegistrationResult.UserRegistrationFailed;
                }
            }

            userAdded = true;
        }

        // Try to add client to user
        if (!await user.TryAddClientAsync(client, cancellationToken))
        {
            RemoveClient(clientId);
            return RegistrationResult.ClientRegistrationFailed;
        }

        return userAdded ? RegistrationResult.OkUserOnline : RegistrationResult.Ok;
    }

    public enum RemoveClientResult
    {
        Ok,
        OkUserOffline,
        OkUserNotFound,
        ClientNotFound
    }
    public static RemoveClientResult RemoveClient(Guid clientSessionId, CancellationToken cancellationToken = default)
    {
        if (Clients.TryRemove(clientSessionId, out WebSocketClient? client) && client is not null)
        {
            Guid userId = client.UserId;

            if (Users.TryGetValue(userId, out WebSocketUser? user) && user is not null)
            {
                user.TryRemoveClient(clientSessionId, out _);
                if (!user.IsOnline)
                {
                    Users.TryRemove(userId, out _);

                    return RemoveClientResult.OkUserOffline;
                }

                return RemoveClientResult.Ok;
            }

            return RemoveClientResult.OkUserNotFound;
        }

        return RemoveClientResult.ClientNotFound;
    }

    public static Task DisconnectUserClientsAsync(Guid userId, WebSocketCloseStatus closeStatus = DefaultCloseStatus, string reason = DefaultCloseReason, CancellationToken cancellationToken = default)
    {
        if (Users.TryRemove(userId, out WebSocketUser? user) && user is not null)
        {
            return user.DisconnectAllAsync(closeStatus, reason, cancellationToken);
        }

        return Task.CompletedTask;
    }

    public static async Task DisconnectAllClientsAsync(WebSocketCloseStatus closeStatus = DefaultCloseStatus, string reason = DefaultCloseReason, CancellationToken cancellationToken = default)
    {
        List<Task> disconnectTasks = new();
        foreach (KeyValuePair<Guid, WebSocketUser> user in Users)
        {
            Users.TryRemove(user.Key, out _);
            disconnectTasks.Add(user.Value.DisconnectAllAsync(closeStatus, reason, cancellationToken));
        }

        await Task.WhenAll(disconnectTasks);
    }

    public static Task RunActionOnClientAsync(Guid clientSessionId, Func<WebSocketClient, Task> action)
    {
        if (Clients.TryGetValue(clientSessionId, out WebSocketClient? client) && client is not null)
        {
            return action(client);
        }

        return Task.CompletedTask;
    }

    public static Task RunActionOnUserAsync(Guid userId, Func<WebSocketClient, Task> action)
    {
        if (Users.TryGetValue(userId, out WebSocketUser? user) && user is not null)
        {
            return user.RunActionOnAllClientsAsync(action);
        }

        return Task.CompletedTask;
    }

    public static async Task RunActionOnAllClientsAsync(Func<WebSocketClient, Task> action)
    {
        foreach (var client in Clients.Values)
        {
            await action(client);
        }
    }
}