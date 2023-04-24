using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Account.Models;

public readonly struct UpdateProfilePictureOk
{
    /// <summary>
    /// 
    /// </summary>
    public Guid ImageId { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required Uri ImageUrl { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required string ImageHash { get; init; }
}
