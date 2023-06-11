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
    public Uri ImageUrl { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public string ImageHash { get; init; }
}
