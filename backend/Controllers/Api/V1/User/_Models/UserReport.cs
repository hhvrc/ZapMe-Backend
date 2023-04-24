using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.User.Models;

/// <summary>
/// Message sent to server to report a user
/// </summary>
public readonly struct UserReport
{
    public Guid UserId { get; init; }

    /// <summary/>
    public string Title { get; init; }

    /// <summary/>
    public string Explenation { get; init; }
}