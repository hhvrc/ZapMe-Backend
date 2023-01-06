using System.Reflection;

namespace ZapMe.Constants;

public static class App
{
    public const string AppName = "ZapMe";
    public const string AppCreator = "HeavenVR";
    public const string MadeInText = "Made in Norway ❤️";

    public static class AppVersion
    {
        public const int Major = 1;
        public const int Minor = 0;
        public const int Build = 0;
        public const int Revision = 0;
        public static readonly Version Version = new Version(Major, Minor, Build, Revision);
        public static readonly string String = Version.ToString();
    }

    public const string AppDescription = "Open source application to control things";
    public const string WebsiteUrl = "https://heavenvr.tech/";
    public const string BackendBaseUrl = "https://heavenvr.tech/zapme";

    public const string MainPageUrl = BackendBaseUrl;

    public const string TermsOfServiceUrl = BackendBaseUrl + "/tos";

    public const string ContactText = "Contact me";
    public const string ContactUrl = BackendBaseUrl + "/contact";

    public const string LicenseText = "The Apache License, Version 2.0";
    public const string LicenseUrl = BackendBaseUrl + "/license";

    public const string PrivacyPolicyUrl = BackendBaseUrl + "/privacy";

    public const string LoginCookieName = AppName + ".Login";
    public const string DevelopmentCorsPolicyName = AppName + ".AllowAll";

    public static readonly string AssemblyXmlPath = Path.Combine(AppContext.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name + ".xml");
}
