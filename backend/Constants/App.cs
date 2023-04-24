using System.Net.Mail;
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
    public const string Domain = "zapme.app";
    public const string WebsiteUrl = "https://" + Domain;
    public const string BackendUrl = "https://api." + Domain;

    public const string TermsOfServiceUrl = WebsiteUrl + "/tos";

    public const string ContactText = "Contact me";
    public const string ContactUrl = WebsiteUrl + "/contact";

    public const string LicenseText = "The Apache License, Version 2.0";
    public const string LicenseUrl = WebsiteUrl + "/license";

    public const string PrivacyPolicyUrl = WebsiteUrl + "/privacy";

    public static readonly MailAddress HelloEmailAddress = new MailAddress("hello@" + Domain, "Hello");
    public static readonly MailAddress SystemEmailAddress = new MailAddress("system@" + Domain, "System");
    public static readonly MailAddress SupportEmailAddress = new MailAddress("support@" + Domain, "Support");
    public static readonly MailAddress ContactEmailAddress = new MailAddress("contact@" + Domain, "Contact");
    public static readonly MailAddress PrivacyEmailAddress = new MailAddress("privacy@" + Domain, "Privacy");

    public static readonly string AssemblyXmlPath = Path.Combine(AppContext.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name + ".xml");
}
