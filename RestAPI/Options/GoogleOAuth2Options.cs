namespace ZapMe.Options.OAuth;

public sealed class GoogleOAuth2Options
{
    public const string SectionName = GoogleOptions.SectionName + ":OAuth2";

    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string[] Scopes { get; set; }
    public required PathString CallbackPath { get; set; }
    public required PathString AccessDeniedPath { get; set; }

    public static GoogleOAuth2Options Get(IConfiguration configuration) =>
        configuration.GetRequiredSection(SectionName).Get<GoogleOAuth2Options>()!;
    public static void Register(IServiceCollection services, IConfiguration configuration) =>
        services.AddOptions<GoogleOAuth2Options>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
}
