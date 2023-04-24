namespace ZapMe.Controllers.Api.V1.Account.OAuth.Models;

/// <summary>
/// Request sent to server to add a oauth connection to a account
/// </summary>
public readonly struct AccountOAuthAdd
{
    public string OauthCode { get; init; }
}