namespace ZapMe.Options.OAuth;

public sealed class GitHubOAuth2Options
{
    public const string SectionName = "Github:OAuth2";

    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string[] Scopes { get; set; }
    public required PathString CallbackPath { get; set; }
    public required PathString AccessDeniedPath { get; set; }

    public static GitHubOAuth2Options Get(IConfiguration configuration) =>
        configuration.GetRequiredSection(SectionName).Get<GitHubOAuth2Options>()!;
    public static void Register(IServiceCollection services, IConfiguration configuration) =>
        services.AddOptions<GitHubOAuth2Options>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
}