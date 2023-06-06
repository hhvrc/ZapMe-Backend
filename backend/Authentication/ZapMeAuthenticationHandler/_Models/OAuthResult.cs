using ZapMe.Controllers.Api.V1;

namespace ZapMe.Authentication.Models;

public enum OAuthResultType
{
    /// <summary>
    /// Authentication succeeded, user is signed in.
    /// </summary>
    SignedIn,

    /// <summary>
    /// OAuth2 succeeded, but 2FA is required to authenticate
    /// </summary>
    RequireTwoFactor,

    /// <summary>
    /// User is not registered in the system, the OAuth provider has provided all the information needed to create a new account, but client must provide a password.
    /// <para>Client must call the <see cref="OAuthController.CompleteOAuthAccountCreation"/> endpoint to create a new account.</para>
    /// <para>OAuth Ticket will be provided in the response body, and must be sent back to the server in the <see cref="OAuthController.CompleteOAuthAccountCreation"/> endpoint.</para>
    /// </summary>
    RequireAccountCreation,
}

public sealed record OAuthTicket(string Ticket, DateTime ExpiresAtUtc);
public readonly record struct OAuthResult(OAuthResultType ResultType, SignInOk? SignInOk, OAuthTicket? OAuthTicket);