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
}