using Microsoft.AspNetCore.Authentication;

namespace Microsoft.AspNetCore.Http;

public static class HttpContextExtensions
{
    // TODO: improve this
    public static async Task<IEnumerable<string>> GetOAuthSchemeNamesAsync(this HttpContext context)
    {
        IAuthenticationSchemeProvider schemesProvider = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

        return (await schemesProvider.GetAllSchemesAsync()).Where(scheme => !String.IsNullOrEmpty(scheme.DisplayName)).Select(scheme => scheme.Name);
    }

    public static async Task<bool> IsOAuthProviderSupportedAsync(this HttpContext context, string provider)
    {
        return (await context.GetOAuthSchemeNamesAsync()).Any(schemeName => String.Equals(schemeName, provider, StringComparison.OrdinalIgnoreCase));
    }
}