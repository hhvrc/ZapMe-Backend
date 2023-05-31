namespace ZapMe.Controllers.Api.V1.OAuth.Models;

/// <summary>
/// 
/// </summary>
public readonly struct OAuthProviderList
{
    /// <summary>
    /// 
    /// </summary>
    public IAsyncEnumerable<string> Providers { get; init; }
}