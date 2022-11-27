using System.Text.Json.Serialization;
using ZapMe.Attributes;
using ZapMe.Data.Models;
using ZapMe.Enums;

namespace ZapMe.Controllers.Api.V1.User.Models;

public class UserDto
{
    public UserDto()
    {
        Id = Guid.Empty;
        Username = String.Empty;
        ProfilePictureId = Guid.Empty;
        OnlineStatus = UserOnlineStatus.Offline;
        OnlineStatusText = String.Empty;
        LastOnline = DateTime.MinValue;
        CreatedAt = DateTime.MinValue;
    }
    public UserDto(AccountEntity user)
    {
        Id = user.Id;
        Username = user.Username;
        ProfilePictureId = user.ProfilePictureId;
        OnlineStatus = user.OnlineStatus;
        OnlineStatusText = user.OnlineStatusText;
        LastOnline = user.LastOnline;
        CreatedAt = user.CreatedAt;
    }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Username(false)]
    [JsonPropertyName("username")]
    public string Username { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("profile_picture_id")]
    public Guid ProfilePictureId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("status")]
    public UserOnlineStatus OnlineStatus { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("status_text")]
    public string OnlineStatusText { get; set; }

    /// <summary>
    /// Date this account was created at
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last time this user was online
    /// </summary>
    [JsonPropertyName("last_online")]
    public DateTime LastOnline { get; set; }
}