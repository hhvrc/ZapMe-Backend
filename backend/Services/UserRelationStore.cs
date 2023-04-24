using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class UserRelationStore : IUserRelationStore
{
    private readonly ZapMeContext _dbContext;
    private readonly ILogger<UserRelationStore> _logger;

    public UserRelationStore(ZapMeContext dbContext, ILogger<UserRelationStore> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<UserRelationEntity> CreateAsync(Guid sourceUserId, Guid targetUserId, CancellationToken cancellationToken)
    {
        var userRelation = new UserRelationEntity
        {
            SourceUserId = sourceUserId,
            TargetUserId = targetUserId,
            RelationType = UserRelationType.None,
            SourceUser = null!,
            TargetUser = null!,
        };

        await _dbContext.UserRelations.AddAsync(userRelation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return userRelation;
    }
}