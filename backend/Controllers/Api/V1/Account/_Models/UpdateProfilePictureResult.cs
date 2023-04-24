namespace ZapMe.Controllers.Api.V1.Account._Models;

public readonly struct UpdateProfilePictureResult
{
    /// <summary>
    /// 
    /// </summary>
    public Guid ImageId { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required string ImageUrl { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required string ImageHash { get; init; }
}
