namespace ZapMe.Options;

public sealed class MailGunOptions
{
    public const string SectionName = "MailGun";

    public required string ApiKey { get; set; }
}