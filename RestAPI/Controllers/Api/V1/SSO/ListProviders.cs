using Microsoft.AspNetCore.Mvc;

namespace ZapMe.Controllers.Api.V1;

public partial class SSOController
{
    /// <summary>
    /// Returns a list of supported SSO providers
    /// </summary>
    [HttpGet("providers", Name = "GetSsoProviderlist")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public Task<IEnumerable<string>> ListSSOProviders() =>
        HttpContext.GetOAuthSchemeNamesAsync();
};
