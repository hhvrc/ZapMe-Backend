using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using ZapMe.Database.Models;
using ZapMe.Enums;

namespace ZapMe.Services.Interfaces;

public interface IUserManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> CreateFriendRequestAsync(Guid sourceUserId, Guid targetUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> AcceptFriendRequestAsync(Guid sourceUserId, Guid targetUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> RejectFriendRequestAsync(Guid sourceUserId, Guid targetUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the relation type from one user to another, and sets the opposite relation if needed.
    /// <para>If A befriends B, then B becomes friend with A, this overrides any previous relation.</para>
    /// <para>If A blocks B, then B becomes nothing to A (except if A has B blocked, then the block remains).</para>
    /// <para>If A becomes nothing to B, B becomes nothing to A (except if B has A blocked, then the block remains).</para>
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="relationType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetUserRelationTypeAsync(Guid sourceUserId, Guid targetUserId, UserRelationType relationType, CancellationToken cancellationToken = default);
}