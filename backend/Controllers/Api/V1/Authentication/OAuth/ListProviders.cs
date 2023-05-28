using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Authentication.Models;

namespace ZapMe.Controllers.Api.V1;

public partial class AuthController
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Returns supported OAuth providers</response>
    [HttpGet("o/list", Name = "OAuth List Providers")]
    [ProducesResponseType(typeof(OAuthProviderList), StatusCodes.Status200OK)]
    public OAuthProviderList ListOAuthProviders()
    {
        return new OAuthProviderList
        {
            Providers = HttpContext.GetExternalProvidersAsync().Select(p => p.Name)
        };
    }
};
