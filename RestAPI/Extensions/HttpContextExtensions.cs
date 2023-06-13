using Microsoft.AspNetCore.Authentication;
using ZapMe.Constants;

namespace Microsoft.AspNetCore.Http;

public static class HttpContextExtensions
{
    public static async Task<IEnumerable<string>> GetOAuthSchemeNamesAsync(this HttpContext context)
    {
        IAuthenticationSchemeProvider schemesProvider = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

        return (await schemesProvider.GetAllSchemesAsync()).Where(scheme => AuthSchemes.OAuth2.Contains(scheme.Name)).Select(scheme => scheme.Name);
    }

    public static async Task<bool> IsOAuthProviderSupportedAsync(this HttpContext context, string provider)
    {
        return AuthSchemes.OAuth2.Contains(provider) && (await context.GetOAuthSchemeNamesAsync()).Contains(provider);
    }
}