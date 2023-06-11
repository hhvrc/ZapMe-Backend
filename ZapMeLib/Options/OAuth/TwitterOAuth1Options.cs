namespace ZapMe.Options.OAuth;

public sealed class TwitterOAuth1Options
{
    public const string SectionName = "Twitter:OAuth1";

    public required string ConsumerKey { get; set; }
    public required string ConsumerSecret { get; set; }
    public required PathString CallbackPath { get; set; }
    public required PathString AccessDeniedPath { get; set; }

    public static TwitterOAuth1Options Get(IConfiguration configuration) =>
        configuration.GetRequiredSection(SectionName).Get<TwitterOAuth1Options>()!;
    public static void Register(IServiceCollection services, IConfiguration configuration) =>
        services.AddOptions<TwitterOAuth1Options>().Bind(configuration.GetRequiredSection(SectionName)).ValidateOnStart();
}
