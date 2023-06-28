namespace ZapMe.Enums.Errors;

public enum FetchClaimsError
{
    DiscordClaimsMissing,
    GithubClaimsMissing,
    TwitterClaimsMissing,
    GoogleClaimsMissing,
    UnsupportedSSOProvider,
}
