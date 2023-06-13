using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.Enums;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class UserRelationStore : IUserRelationStore
{
    private readonly DatabaseContext _dbContext;
    private readonly ILogger<UserRelationStore> _logger;

    public UserRelationStore(DatabaseContext dbContext, ILogger<UserRelationStore> logger)
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