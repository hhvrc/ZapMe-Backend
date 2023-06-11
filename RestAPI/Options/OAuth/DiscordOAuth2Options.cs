namespace ZapMe.Options.OAuth;

public sealed class DiscordOAuth2Options
{
    public const string SectionName = "Discord:OAuth2";

    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string[] Scopes { get; set; }
    public required PathString CallbackPath { get; set; }
    public required PathString AccessDeniedPath { get; set; }

    public static DiscordOAuth2Options Get(IConfiguration configuration) =>
        configuration.GetRequiredSection(SectionName).Get<DiscordOAuth2Options>()!;
    public static void Register(IServiceCollection services, IConfiguration configuration) =>
        services.AddOptions<DiscordOAuth2Options>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
}