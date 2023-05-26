using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OneOf;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Enums;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class OAuthConnectionManager : IOAuthConnectionManager
{
    private readonly UserStore _userStore;
    private readonly ImageManager _imageManager;
    private readonly ZapMeContext _dbContext;
    private readonly ILogger<OAuthConnectionManager> _logger;

    public OAuthConnectionManager(UserStore userStore, ImageManager imageManager, ZapMeContext dbContext, ILogger<OAuthConnectionManager> logger)
    {
        _userStore = userStore;
        _imageManager = imageManager;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<OneOf<OAuthConnectionEntity, ErrorDetails>> GetOrCreateConnectionAsync(string name, string email, ImageEntity? profilePicture, string provider, string providerId, CancellationToken cancellationToken)
    {
        using var transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

        UserEntity? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user == null)
        {
            user = new UserEntity()
            {
                Name = name,
                Email = email,
                EmailVerified = true,
                PasswordHash = "", // TODO: URGENT: fixme
                AcceptedPrivacyPolicyVersion = 1,
                AcceptedTermsOfServiceVersion = 1,
                ProfilePictureId = profilePicture?.Id,
                OnlineStatus = UserStatus.Online,
                OnlineStatusText = ""
            };

            bool created = await _userStore.TryCreateAsync(user, cancellationToken);
            if (!created)
            {
                return CreateHttpError.InternalServerError();
            }
        }

        OAuthConnectionEntity? connectionEntity = await _dbContext.OAuthConnections.FirstOrDefaultAsync(c => c.UserId == user.Id, cancellationToken);
        if (connectionEntity == null)
        {
            connectionEntity = new OAuthConnectionEntity
            {
                UserId = user.Id,
                User = user,
                ProviderName = provider,
                ProviderId = providerId,
            };

            await _dbContext.OAuthConnections.AddAsync(connectionEntity, cancellationToken);
        }

        if (transaction != null)
        {
            await transaction.CommitAsync(cancellationToken);
        }

        return connectionEntity;
    }
}