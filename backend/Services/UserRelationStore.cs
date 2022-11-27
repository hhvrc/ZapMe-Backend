using Microsoft.EntityFrameworkCore;
using Npgsql;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class UserRelationStore : IUserRelationStore
{
    private readonly ZapMeContext _dbContext;
    private readonly IHybridCache _cache;
    private readonly ILogger<UserRelationStore> _logger;

    public UserRelationStore(ZapMeContext dbContext, IHybridCache cacheProviderService, ILogger<UserRelationStore> logger)
    {
        _dbContext = dbContext;
        _cache = cacheProviderService;
        _logger = logger;
    }

    public async Task<UserRelationEntity?> CreateAsync(Guid sourceUserId, Guid targetUserId, CancellationToken cancellationToken)
    {
        try
        {
            var entity = new UserRelationEntity
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId,
                RelationType = UserRelationType.None,
                SourceUser = null!,
                TargetUser = null!,
            };

            await _dbContext.UserRelations.AddAsync(entity, cancellationToken);

            return entity;
        }
        catch (PostgresException)
        {
        }

        return null;
    }

    public Task<UserRelationEntity[]> ListOutgoingAsync(Guid sourceUserId, CancellationToken cancellationToken)
    {
        return _dbContext.UserRelations.Where(ur => ur.SourceUserId == sourceUserId).ToArrayAsync(cancellationToken);
    }
    
    public Task<UserRelationEntity[]> ListIncomingByTypeAsync(Guid targetUserId, UserRelationType relationType, CancellationToken cancellationToken)
    {
        return _dbContext.UserRelations.Where(ur => ur.TargetUserId == targetUserId && ur.RelationType == relationType).ToArrayAsync(cancellationToken);
    }

    public async Task<bool> SetRelationTypeAsync(Guid sourceUserId, Guid targetUserId, UserRelationType relationType, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRelations
            .Where(ur => ur.SourceUserId == sourceUserId && ur.TargetUserId == targetUserId)
            .ExecuteUpdateAsync(s => s.SetProperty(static ur => ur.RelationType, _ => relationType), cancellationToken) > 0;
    }

    public async Task<bool> SetNickNameAsync(Guid sourceUserId, Guid targetUserId, string nickName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRelations
            .Where(ur => ur.SourceUserId == sourceUserId && ur.TargetUserId == targetUserId)
            .ExecuteUpdateAsync(s => s.SetProperty(static ur => ur.NickName, _ => nickName), cancellationToken) > 0;
    }

    public async Task<bool> SetNotesAsync(Guid sourceUserId, Guid targetUserId, string notes, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRelations
            .Where(ur => ur.SourceUserId == sourceUserId && ur.TargetUserId == targetUserId)
            .ExecuteUpdateAsync(s => s.SetProperty(static ur => ur.Notes, _ => notes), cancellationToken) > 0;
    }
    
    public async Task<bool> DeleteAsync(Guid sourceUserId, Guid targetUserId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRelations.Where(ur => ur.SourceUserId == sourceUserId && ur.TargetUserId == targetUserId).ExecuteDeleteAsync(cancellationToken) > 0;
    }

    public Task<int> PurgeAsync(Guid userId, UserRelationType? relationType, CancellationToken cancellationToken)
    {
        return _dbContext.UserRelations.Where(ur => ur.SourceUserId == userId || ur.TargetUserId == userId).ExecuteDeleteAsync(cancellationToken);
    }
}