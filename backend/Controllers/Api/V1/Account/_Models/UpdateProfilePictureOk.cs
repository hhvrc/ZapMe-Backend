using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Account.Models;

public readonly struct UpdateProfilePictureOk
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("imageId")]
    public Guid ImageId { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("imageUrl")]
    public required Uri ImageUrl { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("imageHash")]
    public required string ImageHash { get; init; }
}
