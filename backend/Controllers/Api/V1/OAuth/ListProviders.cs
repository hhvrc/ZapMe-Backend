using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.OAuth.Models;

namespace ZapMe.Controllers.Api.V1;

public partial class OAuthController
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Returns supported OAuth providers</response>
    [HttpGet("list", Name = "OAuth List Providers")]
    [ProducesResponseType(typeof(OAuthProviderList), StatusCodes.Status200OK)]
    public OAuthProviderList ListOAuthProviders()
    {
        return new OAuthProviderList
        {
            Providers = HttpContext.GetExternalProvidersAsync().Select(p => p.Name)
        };
    }
};
