using Microsoft.AspNetCore.Authentication;
using ZapMe.Data.Models;

namespace Microsoft.AspNetCore.Http;

public static class HttpContextExtensions
{
    public static SignInEntity? GetSignIn(this HttpContext context) =>
        context.Items["SignIn"] as SignInEntity;
    public static void SetSignIn(this HttpContext context, SignInEntity signIn) =>
        context.Items["SignIn"] = signIn;

    public static async Task<AuthenticationScheme[]> GetExternalProvidersAsync(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

        return (from scheme in await schemes.GetAllSchemesAsync()
                where !String.IsNullOrEmpty(scheme.DisplayName)
                select scheme).ToArray();
    }

    public static async Task<bool> IsProviderSupportedAsync(this HttpContext context, string provider)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        if (String.IsNullOrWhiteSpace(provider))
        {
            return false;
        }

        return (from scheme in await context.GetExternalProvidersAsync()
                where String.Equals(scheme.Name, provider)
                select scheme).Any();
    }
    public static string GetRemoteIP(this HttpContext context)
    {
        if (!context.Items.TryGetValue("RequestIpAddress", out object? obj) && obj is string ipAddr)
        {
            return ipAddr;
        }

        ipAddr = context.Request.Headers.GetPrefferedHeader("CF-Connecting-IP", "X-Forwarded-For", "X-Real-IP", "True-Client-IP", "X-Client-IP").FirstOrDefault()
            ?? context.Connection?.RemoteIpAddress?.ToString()
            ?? throw new NullReferenceException("Unable to get any IP address, this should never happen"); // This should never happen, at least it should return localhost

        context.Items["RequestIpAddress"] = ipAddr;

        return ipAddr;
    }
}