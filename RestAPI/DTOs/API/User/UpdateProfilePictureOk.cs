namespace ZapMe.DTOs.API.User;

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
