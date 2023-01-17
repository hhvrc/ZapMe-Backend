using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ZapMe.Attributes;

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