﻿using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using ZapMe.Services.Interfaces;
using ZapMe.Websocket;

namespace ZapMe.Services;

public sealed class WebSocketInstanceManager : IWebSocketInstanceManager
{
    private static readonly ConcurrentDictionary<string, WebSocketInstance> _Instances = new();

    private readonly ILogger<WebSocketInstanceManager> _logger;

    public WebSocketInstanceManager(ILogger<WebSocketInstanceManager> logger)
    {
        _logger = logger;
    }

    public async Task<bool> RegisterInstanceAsync(Guid userId, string instanceId, WebSocketInstance instance, CancellationToken cancellationToken)
    {
        if (!_Instances.TryAdd(instanceId, instance))
        {
            return false;
        }

        try
        {
            // Add to redis

            // Publish event
        }
        catch
        {
            _Instances.Remove(instanceId, out _);
            await instance.CloseAsync(WebSocketCloseStatus.InternalServerError, "Failed to register instance", cancellationToken);
            throw;
        }

        return true;
    }

    public async Task RemoveInstanceAsync(string instanceId, string reason, CancellationToken cancellationToken)
    {
        if (_Instances.Remove(instanceId, out var instance) && instance is not null)
        {
            await instance.CloseAsync(WebSocketCloseStatus.NormalClosure, reason, cancellationToken);

            // Remove from redis
        }
    }

    public async Task RemoveAllInstancesAsync(Guid userId, string reason, CancellationToken cancellationToken)
    {
        foreach (var instance in _Instances.Where(x => x.Value.UserId == userId).ToArray()) // ToArray is very important here to avoid collection modified exception
        {
            await RemoveInstanceAsync(instance.Key, reason, cancellationToken);
        }
    }
}