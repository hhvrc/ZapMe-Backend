namespace ZapMe.Options;

public sealed class ZapMeAuthenticationOptions
{
    public const string SectionName = "Authentication";

    /// <summary>
    /// Name of the cookie which will be sent to client
    /// </summary>
    public required string CookieName { get; set; }

    /// <summary>
    /// Sets if the cookie should be renewed if its half way through the expiration time.
    /// </summary>
    public required bool SlidingExpiration { get; set; }

    public static void Register(IServiceCollection services, IConfiguration configuration) =>
        services.AddOptions<ZapMeAuthenticationOptions>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
}