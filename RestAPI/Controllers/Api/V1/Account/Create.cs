using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ZapMe.Attributes;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.DTOs.API.User;
using ZapMe.Enums;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Create a new account
    /// </summary>
    /// <param name="body"></param>
    /// <param name="cfTurnstileService"></param>
    /// <param name="debounceService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="201">Created account</response>
    [AnonymousOnly]
    [RequestSizeLimit(1024)]
    [HttpPost(Name = "CreateAccount")]
    [ProducesResponseType(typeof(CreateOk), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Invalid SSO token
    [ProducesResponseType(StatusCodes.Status409Conflict)] // Username/email already taken
    public async Task<IActionResult> Create(
        [FromBody] CreateAccount body,
        [FromServices] ICloudflareTurnstileService cfTurnstileService,
        [FromServices] IDebounceService debounceService,
        CancellationToken cancellationToken
        )
    {
        SSOProviderDataEntry? providerVariables = null;
        if (!String.IsNullOrEmpty(body.SSOToken))
        {
            var stateStore = HttpContext.RequestServices.GetRequiredService<ISSOStateStore>();
            providerVariables = await stateStore.GetProviderDataAsync(body.SSOToken, this.GetRemoteIP(), cancellationToken);
            if (providerVariables is null)
            {
                return HttpErrors.InvalidSSOTokenActionResult;
            }
        }

        // Verify turnstile token
        CloudflareTurnstileVerifyResponse reCaptchaResponse = await cfTurnstileService.VerifyUserResponseTokenAsync(body.TurnstileResponse, this.GetRemoteIP(), cancellationToken);
        if (!reCaptchaResponse.Success)
        {
            if (reCaptchaResponse.ErrorCodes is not null)
            {
                foreach (string errorCode in reCaptchaResponse.ErrorCodes)
                {
                    switch (errorCode)
                    {
                        case "missing-input-response": // The response parameter was not passed.
                            return HttpErrors.InvalidModelState((nameof(body.TurnstileResponse), "Missing Cloudflare Turnstile Response")).ToActionResult();
                        case "invalid-input-response": // The response parameter is invalid or has expired.
                            return HttpErrors.InvalidModelState((nameof(body.TurnstileResponse), "Invalid Cloudflare Turnstile Response")).ToActionResult();
                        case "timeout-or-duplicate": // The response parameter has already been validated before.
                            return HttpErrors.InvalidModelState((nameof(body.TurnstileResponse), "Cloudflare Turnstile Response Expired or Already Used")).ToActionResult();
                        case "missing-input-secret": // The secret parameter was not passed.
                            _logger.LogError("Missing Cloudflare Turnstile Secret");
                            break;
                        case "invalid-input-secret": // The secret parameter was invalid or did not exist.
                            _logger.LogError("Invalid Cloudflare Turnstile Secret");
                            break;
                        case "bad-request": // The request was rejected because it was malformed.
                            _logger.LogError("Bad Cloudflare Turnstile Request");
                            break;
                        case "internal-error": // An internal error happened while validating the response. The request can be retried.
                            _logger.LogError("Cloudflare Turnstile Internal Error");
                            break;
                        default:
                            break;
                    };
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        // Attempt to check against debounce if the email is a throwaway email
        if (await debounceService.IsDisposableEmailAsync(body.Email, cancellationToken))
        {
            return HttpErrors.InvalidModelState((nameof(body.Email), "Disposable Emails are not allowed")).ToActionResult();
        }

        uint privacyPolicyVersion = await _dbContext.PrivacyPolicyDocuments.Where(pp => pp.IsActive).Select(pp => pp.Version).OrderByDescending(pp => pp).FirstAsync(cancellationToken);
        if (body.AcceptedPrivacyPolicyVersion < privacyPolicyVersion)
        {
            return HttpErrors.ReviewPrivacyPolicyActionResult;
        }

        uint termsOfServiceVersion = await _dbContext.TermsOfServiceDocuments.Where(tos => tos.IsActive).Select(tos => tos.Version).OrderByDescending(tos => tos).FirstAsync(cancellationToken);
        if (body.AcceptedTermsOfServiceVersion < termsOfServiceVersion)
        {
            return HttpErrors.ReviewTermsOfServiceActionResult;
        }

        await using ScopedDelayLock tl = ScopedDelayLock.FromSeconds(2, cancellationToken);

        if (_dbContext.Users.Any(u => u.Name == body.Username || u.Email == body.Email))
        {
            return HttpErrors.UserNameOrEmailTakenActionResult;
        }

        using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        ImageEntity? avatarImageEntity = null;
        ImageEntity? bannerImageEntity = null;
        if (providerVariables is not null)
        {
            var imageManager = HttpContext.RequestServices.GetRequiredService<IImageManager>();
            if (!String.IsNullOrEmpty(providerVariables.ProviderAvatarUrl))
            {
                var getOrCreateImageResult = await imageManager.GetOrCreateRecordAsync(
                    providerVariables.ProviderAvatarUrl,
                    CountryRegionLookup.GetCloudflareRegion(this.GetCloudflareIPCountry()),
                    null,
                    null,
                    cancellationToken
                    );
                if (getOrCreateImageResult.TryPickT1(out ErrorDetails errorDetails, out avatarImageEntity))
                {
                    return errorDetails.ToActionResult();
                }
            }
            if (!String.IsNullOrEmpty(providerVariables.ProviderBannerUrl))
            {
                var getOrCreateImageResult = await imageManager.GetOrCreateRecordAsync(
                    providerVariables.ProviderBannerUrl,
                    CountryRegionLookup.GetCloudflareRegion(this.GetCloudflareIPCountry()),
                    null,
                    null,
                    cancellationToken
                    );
                if (getOrCreateImageResult.TryPickT1(out ErrorDetails errorDetails, out bannerImageEntity))
                {
                    return errorDetails.ToActionResult();
                }
            }
        }

        // If a OAuth provider has verified the email, and the user has not changed it, then we can mark the email as verified
        bool emailVerified = providerVariables?.ProviderUserEmail == body.Email && providerVariables.ProviderUserEmailVerified;

        UserEntity user = new UserEntity
        {
            Name = body.Username,
            Email = body.Email,
            EmailVerified = emailVerified,
            PasswordHash = PasswordUtils.HashPassword(body.Password),
            AcceptedPrivacyPolicyVersion = body.AcceptedPrivacyPolicyVersion,
            AcceptedTermsOfServiceVersion = body.AcceptedTermsOfServiceVersion,
            ProfileAvatarId = avatarImageEntity?.Id,
            ProfileBannerId = bannerImageEntity?.Id,
            Status = UserStatus.Online,
            StatusText = String.Empty
        };

        // Create account
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (avatarImageEntity is not null)
        {
            await _dbContext.Images
                .Where(i => i.Id == avatarImageEntity.Id)
                .ExecuteUpdateAsync(spc => spc.SetProperty(i => i.UploaderId, _ => user.Id), cancellationToken);
        }
        if (bannerImageEntity is not null)
        {
            await _dbContext.Images
                .Where(i => i.Id == bannerImageEntity.Id)
                .ExecuteUpdateAsync(spc => spc.SetProperty(i => i.UploaderId, _ => user.Id), cancellationToken);
        }

        string? jwtToken = null;
        if (providerVariables is not null)
        {
            var connectionEntity = new SSOConnectionEntity
            {
                UserId = user.Id,
                ProviderName = providerVariables.ProviderName,
                ProviderUserId = providerVariables.ProviderUserId,
                ProviderUserName = providerVariables.ProviderUserName,
            };

            _dbContext.SSOConnections.Add(connectionEntity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var sessionManager = HttpContext.RequestServices.GetRequiredService<ISessionManager>();
            SessionEntity? session = await sessionManager.CreateAsync(
                user,
                this.GetRemoteIP(),
                this.GetCloudflareIPCountry(),
                this.GetRemoteUserAgent(),
                false,
                cancellationToken
                );

            var authenticationManager = HttpContext.RequestServices.GetRequiredService<IJwtAuthenticationManager>();

            jwtToken = authenticationManager.GenerateJwtToken(session.ToClaimsIdentity(), session.CreatedAt, session.ExpiresAt);
        }

        // Send email verification
        if (!emailVerified)
        {
            var emailVerificationManager = HttpContext.RequestServices.GetRequiredService<IEmailVerificationManager>();
            ErrorDetails? test = await emailVerificationManager.InitiateEmailVerificationAsync(user, body.Email, cancellationToken);
            if (test.HasValue)
            {
                return test.Value.ToActionResult();
            }
        }

        // Commit transaction
        await transaction.CommitAsync(cancellationToken);

        return CreatedAtAction(nameof(Get), new CreateOk
        {
            AccountId = user.Id,
            Session = jwtToken is null ? null : new AuthenticationResponse(jwtToken),
            EmailVerificationRequired = !emailVerified
        });
    }
}