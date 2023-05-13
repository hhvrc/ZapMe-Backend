﻿namespace ZapMe.Options;

public sealed class ZapMeOptions
{
    public required uint PrivacyPolicyVersion { get; set; }
    public required uint TermsOfServiceVersion { get; set; }
    public required ZapMeAuthenticationOptions Authentication { get; set; }
    public required DatabaseOptions Database { get; set; }
    public required CloudflareOptions Cloudflare { get; set; }
    public required GoogleOptions Google { get; set; }
    public required MailGunOptions MailGun { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ZapMeOptions>().Bind(configuration).ValidateOnStart();

        ZapMeAuthenticationOptions.Register(services, configuration);
        CloudflareOptions.Register(services, configuration);
        DatabaseOptions.Register(services, configuration);
        GoogleOptions.Register(services, configuration);
        MailGunOptions.Register(services, configuration);
    }
}