namespace ZapMe.Options;

public sealed class CloudflareOptions
{
    public const string SectionName = "Cloudflare";

    public required CloudflareTurnstileOptions Turnstile { get; set; }
}

public sealed class CloudflareTurnstileOptions
{
    public const string SectionName = CloudflareOptions.SectionName + ":Turnstile";

    public required string SiteKey { get; set; }
    public required string SecretKey { get; set; }
}