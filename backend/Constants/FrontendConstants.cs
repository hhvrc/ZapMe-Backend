namespace ZapMe.Constants;

public static class FrontendConstants
{
    public const string FrontendProdUrl = "https://zapme.app";
    public const string FrontendDevUrl = "http://localhost:1337";
    public static readonly string[] NonFrontendPathSegments = new string[] { "/api", "/swagger" };
}
