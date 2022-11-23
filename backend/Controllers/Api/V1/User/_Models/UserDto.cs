using System.Text.Json.Serialization;
using ZapMe.Attributes;
using ZapMe.Records;

namespace ZapMe.Controllers.Api.V1.User.Models;

public class UserDto : IUserRecord
{
    public UserDto()
    {
        Id = Guid.Empty;
        UserName = String.Empty;
        ProfilePictureId = Guid.Empty;
        OnlineStatus = UserOnlineStatus.Offline;
        OnlineStatusText = String.Empty;
        LastOnline = DateTime.MinValue;
        CreatedAt = DateTime.MinValue;
    }
    public UserDto(IUserRecord user)
    {
        Id = user.Id;
        UserName = user.UserName;
        ProfilePictureId = user.ProfilePictureId;
        OnlineStatus = user.OnlineStatus;
        OnlineStatusText = user.OnlineStatusText;
        LastOnline = user.LastOnline;
        CreatedAt = user.CreatedAt;
    }

    /// <inheritdoc/>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <inheritdoc/>
    [Username(false)]
    [JsonPropertyName("username")]
    public string UserName { get; set; }

    /// <inheritdoc/>
    [JsonPropertyName("profile_picture_id")]
    public Guid ProfilePictureId { get; set; }

    /// <inheritdoc/>
    [JsonPropertyName("status")]
    public UserOnlineStatus OnlineStatus { get; set; }

    /// <inheritdoc/>
    [JsonPropertyName("status_text")]
    public string OnlineStatusText { get; set; }

    /// <inheritdoc/>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc/>
    [JsonPropertyName("last_online")]
    public DateTime LastOnline { get; set; }
}
