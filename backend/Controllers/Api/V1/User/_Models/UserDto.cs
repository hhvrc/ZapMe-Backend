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
        OnlineStatus = UserStatus.Offline;
        OnlineStatusText = String.Empty;
        CreatedAt = DateTime.MinValue;
        LastOnline = DateTime.MinValue;
    }
    public UserDto(UserEntity user)
    {
        Id = user.Id;
        Username = user.Name;
        ProfilePictureId = user.ProfilePictureId;
        OnlineStatus = user.OnlineStatus;
        OnlineStatusText = user.OnlineStatusText;
        CreatedAt = user.CreatedAt;
        LastOnline = user.LastOnline;
    }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyOrder(0)]
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [Username(false)]
    [JsonPropertyOrder(1)]
    [JsonPropertyName("username")]
    public string Username { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyOrder(5)]
    [JsonPropertyName("profile_picture_id")]
    public Guid ProfilePictureId { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyOrder(8)]
    [JsonPropertyName("status")]
    public UserStatus OnlineStatus { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyOrder(9)]
    [JsonPropertyName("status_text")]
    public string OnlineStatusText { get; init; }

    /// <summary>
    /// Date this user was created at
    /// </summary>
    [JsonPropertyOrder(15)]
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last time this user was online
    /// </summary>
    [JsonPropertyOrder(16)]
    [JsonPropertyName("last_online")]
    public DateTime LastOnline { get; init; }
}