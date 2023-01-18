using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Authentication.Models;

/// <summary>
/// 
/// </summary>
public struct OAuthProviderList
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("providers")]
    public IAsyncEnumerable<string> Providers { get; set; }
}