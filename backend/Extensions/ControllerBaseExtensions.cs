namespace Microsoft.AspNetCore.Mvc;

public static class ControllerBaseExtensions
{
    /// <summary>
    /// Tries to get the remote IP address of the client based on multiple headers with a fallback to <see cref="HttpContext"/>'s Remote IP.
    /// </summary>
    /// <param name="controllerBase"></param>
    /// <returns></returns>
    public static string GetRemoteIP(this ControllerBase controllerBase) =>
        controllerBase.HttpContext.GetRemoteIP();

    /// <summary>
    /// Uses Cloudflare's cf-ipcontry header to determine the country code of the request.
    /// </summary>
    /// <param name="controllerBase"></param>
    /// <returns>A 2 character country code, or null if the header is not present.</returns>
    public static string GetCloudflareIPCountry(this ControllerBase controllerBase) =>
        controllerBase.HttpContext.GetCloudflareIPCountry();

    /// <summary>
    /// Returns the User-Agent header of the request.
    /// </summary>
    /// <param name="controllerBase"></param>
    /// <returns></returns>
    public static string GetRemoteUserAgent(this ControllerBase controllerBase) =>
        controllerBase.HttpContext.GetRemoteUserAgent();
}