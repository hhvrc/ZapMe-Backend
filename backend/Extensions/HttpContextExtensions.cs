using Microsoft.AspNetCore.Authentication;

namespace Microsoft.AspNetCore.Http;

public static class HttpContextExtensions
{
    public static async IAsyncEnumerable<AuthenticationScheme> GetExternalProvidersAsync(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        IAuthenticationSchemeProvider schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

        foreach (AuthenticationScheme? scheme in from scheme in await schemes.GetAllSchemesAsync() where !String.IsNullOrEmpty(scheme.DisplayName) select scheme)
        {
            yield return scheme;
        }
    }

    public static ValueTask<bool> IsProviderSupportedAsync(this HttpContext context, string provider)
    {
        return GetExternalProvidersAsync(context).AnyAsync(scheme => String.Equals(scheme.Name, provider, StringComparison.OrdinalIgnoreCase));
    }

    public static string GetRemoteIP(this HttpContext context)
    {
        if (!context.Items.TryGetValue("RequestIpAddress", out object? obj) && obj is string ipAddr)
        {
            return ipAddr;
        }

        ipAddr = context.Request.Headers.GetPrefferedHeader("CF-Connecting-IP", "True-Client-IP", "X-Forwarded-For", "X-Real-IP", "X-Client-IP").FirstOrDefault() // Note: The order is important, and it is assumed that this server will be behind cloudflare
            ?? context.Connection?.RemoteIpAddress?.ToString()
            ?? throw new NullReferenceException("Unable to get any IP address, this should never happen"); // This should never happen, at least it should return localhost

        context.Items["RequestIpAddress"] = ipAddr;

        return ipAddr;
    }

    public static string GetCloudflareIPCountry(this HttpContext context)
    {
        return context.Request.Headers.GetFirst("cf-ipcountry") ?? "ZZ";
    }

    public static string GetRemoteUserAgent(this HttpContext context)
    {
        return context.Request.Headers.GetFirst("User-Agent") ?? "Unknown";
    }
}