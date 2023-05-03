namespace ZapMe.Options;

public sealed class ZapMeOptions
{
    public required ZapMeAuthenticationOptions Authentication { get; set; }
    public required DatabaseOptions Database { get; set; }
    public required CloudflareOptions Cloudflare { get; set; }
    public required DiscordOptions Discord { get; set; }
    public required GithubOptions Github { get; set; }
    public required GoogleOptions Google { get; set; }
    public required MailGunOptions MailGun { get; set; }
    public required TwitterOptions Twitter { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ZapMeOptions>().ValidateOnStart();

        ZapMeAuthenticationOptions.Register(services, configuration);
        CloudflareOptions.Register(services, configuration);
        DatabaseOptions.Register(services, configuration);
        DiscordOptions.Register(services, configuration);
        GithubOptions.Register(services, configuration);
        GoogleOptions.Register(services, configuration);
        MailGunOptions.Register(services, configuration);
        TwitterOptions.Register(services, configuration);
    }
}