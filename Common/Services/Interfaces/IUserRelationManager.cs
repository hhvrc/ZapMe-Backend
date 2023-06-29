using ZapMe.DTOs;
using ZapMe.Enums;

namespace ZapMe.Services.Interfaces;

public interface IUserRelationManager
{
    /// <summary>
    /// Will create a friend request to the target user if one does not already exist, or accept the friend request from the target user if one does exist.
    /// </summary>
    Task<CreateOrAcceptFriendRequestResult> CreateOrAcceptFriendRequestAsync(Guid fromUserId, Guid toUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Will delete a friend request to the target user if one exists, or reject the friend request from the target user if one exist.
    /// </summary>
    Task<UpdateUserRelationResult> DeleteOrRejectFriendRequestAsync(Guid fromUserId, Guid toUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Will remove the friendship between the two users, will also remove any friend requests between the two users.
    /// </summary>
    Task<UpdateUserRelationResult> RemoveFriendshipAsync(Guid fromUserId, Guid toUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Will apply a block to the target user if one does not already exist, and remove any friendship or friend requests between the two users.
    /// </summary>
    Task<UpdateUserRelationResult> BlockUserAsync(Guid fromUserId, Guid toUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Will remove the block to the target user if one exists, will not effect any block applied by the target user.
    /// </summary>
    Task<UpdateUserRelationResult> UnblockUserAsync(Guid fromUserId, Guid toUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the relation type from one user to another, and sets the opposite relation if needed.
    /// </summary>
    Task<UpdateUserRelationResult> SetUserRelationDetailsAsync(Guid fromUserId, Guid toUserId, SetUserRelationDto relationUpdate, CancellationToken cancellationToken = default);
}