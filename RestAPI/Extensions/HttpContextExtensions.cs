using Microsoft.AspNetCore.Authentication;
using ZapMe.Constants;

namespace Microsoft.AspNetCore.Http;

public static class HttpContextExtensions
{
    public static string GetRemoteIP(this HttpContext context)
    {
        if (!context.Items.TryGetValue("RequestIpAddress", out object? obj) && obj is string ipAddr)
        {
            return ipAddr;
        }

        // Note: The order is important, and it is assumed that this server will be behind cloudflare
        ipAddr = context.Request.Headers.GetPrefferedHeader("CF-Connecting-IP", "True-Client-IP", "X-Forwarded-For", "X-Real-IP", "X-Client-IP")
            ?? context.Connection?.RemoteIpAddress?.ToString()
            ?? throw new NullReferenceException("Unable to get any IP address, this should never happen"); // This should never happen, at least it should return localhost

        context.Items["RequestIpAddress"] = ipAddr;

        return ipAddr;
    }

    public static string GetCloudflareIPCountry(this HttpContext context)
    {
        return (string?)context.Request.Headers["CF-IPCountry"] ?? "ZZ";
    }

    public static string GetRemoteUserAgent(this HttpContext context)
    {
        return (string?)context.Request.Headers["User-Agent"] ?? "Unknown";
    }

    public static async Task<IEnumerable<string>> GetOAuthSchemeNamesAsync(this HttpContext context)
    {
        IAuthenticationSchemeProvider schemesProvider = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

        return (await schemesProvider.GetAllSchemesAsync()).Where(scheme => AuthenticationConstants.OAuth2Schemes.Contains(scheme.Name)).Select(scheme => scheme.Name);
    }

    public static async Task<bool> IsOAuthProviderSupportedAsync(this HttpContext context, string provider)
    {
        return AuthenticationConstants.OAuth2Schemes.Contains(provider) && (await context.GetOAuthSchemeNamesAsync()).Contains(provider);
    }
}