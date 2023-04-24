using Microsoft.EntityFrameworkCore;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class EmailVerificationRequestStore : IEmailVerificationRequestStore
{
    private readonly ZapMeContext _context;

    public EmailVerificationRequestStore(ZapMeContext context)
    {
        _context = context;
    }

    public async Task<EmailVerificationRequestEntity> CreateAsync(Guid userId, string newEmail, string tokenHash, CancellationToken cancellationToken)
    {
        EmailVerificationRequestEntity emailVerificationRequest = new EmailVerificationRequestEntity
        {
            UserId = userId,
            NewEmail = newEmail,
            TokenHash = tokenHash
        };

        await _context.EmailVerificationRequests.AddAsync(emailVerificationRequest, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return emailVerificationRequest;
    }
}
