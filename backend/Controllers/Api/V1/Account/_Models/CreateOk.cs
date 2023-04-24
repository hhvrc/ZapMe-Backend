﻿using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Account.Models;

public readonly struct CreateOk
{
    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required string Message { get; init; }
}
