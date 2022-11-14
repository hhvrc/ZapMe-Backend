using Microsoft.EntityFrameworkCore;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class SignInStore : ISignInStore
{
    private readonly ZapMeContext _dbContext;
    private readonly ILogger<SignInStore> _logger;

    public SignInStore(ZapMeContext dbContext, ILogger<SignInStore> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }


    public async Task<SignInEntity?> TryCreateAsync(Guid userId, string deviceName, DateTime expiresAt, CancellationToken cancellationToken)
    {
        var signIn = new SignInEntity
        {
            User = null!, // TODO: wtf do i do now? ask on C# discord
            UserId = userId,
            DeviceName = deviceName,
            ExpiresAt = expiresAt
        };

        await _dbContext.SignIns.AddAsync(signIn, cancellationToken);
        int nAdded = await _dbContext.SaveChangesAsync(cancellationToken);

        if (nAdded > 0)
        {
            return await GetByIdAsync(signIn.Id, cancellationToken);
        }

        return null;
    }

    public async Task<SignInEntity?> GetByIdAsync(Guid signInId, CancellationToken cancellationToken)
    {
        return await _dbContext.SignIns.Include(static s => s.User).FirstOrDefaultAsync(s => s.Id == signInId, cancellationToken);
    }

    public async Task<SignInEntity[]> ListByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.SignIns.Where(s => s.UserId == userId).ToArrayAsync(cancellationToken);
    }

    public async Task<bool> SetExipresAtAsync(Guid signInId, DateTime expiresAt, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.SignIns.Where(s => s.Id == signInId).ExecuteUpdateAsync(s => s.SetProperty(static s => s.ExpiresAt, _ => expiresAt), cancellationToken) > 0;
        }
        catch (DbUpdateException)
        {
        }

        return false;
    }

    public async Task<bool> DeleteAsync(Guid signInId, CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.SignIns.Where(s => s.Id == signInId).ExecuteDeleteAsync(cancellationToken) > 0;
        }
        catch (DbUpdateException)
        {
        }

        return false;
    }

    public async Task<bool> DeleteAllAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.SignIns.Where(s => s.UserId == userId).ExecuteDeleteAsync(cancellationToken) > 0;
        }
        catch (DbUpdateException)
        {
        }

        return false;
    }
}
