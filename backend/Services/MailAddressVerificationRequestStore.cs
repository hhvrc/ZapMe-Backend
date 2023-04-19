using Microsoft.EntityFrameworkCore;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class MailAddressVerificationRequestStore : IMailAddressVerificationRequestStore
{
    private readonly ZapMeContext _context;

    public MailAddressVerificationRequestStore(ZapMeContext context)
    {
        _context = context;
    }

    public async Task<MailAddressChangeRequestEntity> CreateAsync(Guid userId, string newEmail, string tokenHash, CancellationToken cancellationToken = default)
    {
        MailAddressChangeRequestEntity mailAddressVerificationRequest = new MailAddressChangeRequestEntity
        {
            UserId = userId,
            NewEmail = newEmail,
            TokenHash = tokenHash
        };

        await _context.MailAddressVerificationRequests.AddAsync(mailAddressVerificationRequest, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return mailAddressVerificationRequest;
    }

    public Task<MailAddressChangeRequestEntity?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _context.MailAddressVerificationRequests.Include(mavr => mavr.User).FirstOrDefaultAsync(mavr => mavr.UserId == userId, cancellationToken);
    }

    public Task<MailAddressChangeRequestEntity?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return _context.MailAddressVerificationRequests.Include(mavr => mavr.User).FirstOrDefaultAsync(mavr => mavr.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<bool> DeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.MailAddressVerificationRequests.Where(mavr => mavr.UserId == userId).ExecuteDeleteAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await _context.MailAddressVerificationRequests.Where(mavr => mavr.TokenHash == tokenHash).ExecuteDeleteAsync(cancellationToken) > 0;
    }
}
