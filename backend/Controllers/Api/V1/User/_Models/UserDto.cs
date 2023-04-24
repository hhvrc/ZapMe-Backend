using System.Text.Json.Serialization;
using ZapMe.Attributes;
using ZapMe.Data.Models;
using ZapMe.Enums;

namespace ZapMe.Controllers.Api.V1.User.Models;

public readonly struct UserDto
{
    public UserDto()
    {
        Id = Guid.Empty;
        Username = String.Empty;
        ProfilePictureId = Guid.Empty;
        Status = UserStatus.Offline;
        StatusText = String.Empty;
        CreatedAt = DateTime.MinValue;
        LastOnline = DateTime.MinValue;
    }
    public UserDto(UserEntity user)
    {
        Id = user.Id;
        Username = user.Name;
        ProfilePictureId = user.ProfilePictureId;
        Status = user.OnlineStatus;
        StatusText = user.OnlineStatusText;
        CreatedAt = user.CreatedAt;
        LastOnline = user.LastOnline;
    }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyOrder(0)]
    public Guid Id { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [Username(false)]
    [JsonPropertyOrder(1)]
    public string Username { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyOrder(5)]
    public Guid ProfilePictureId { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyOrder(8)]
    public UserStatus Status { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyOrder(9)]
    public string StatusText { get; init; }

    /// <summary>
    /// Date this user was created at
    /// </summary>
    [JsonPropertyOrder(15)]
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last time this user was online
    /// </summary>
    [JsonPropertyOrder(16)]
    public DateTime LastOnline { get; init; }
}