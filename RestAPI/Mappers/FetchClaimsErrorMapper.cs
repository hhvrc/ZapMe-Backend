using ZapMe.DTOs;
using ZapMe.Enums.Errors;

namespace ZapMe.Mappers;

public static class FetchClaimsErrorMapper
{
    public static ErrorDetails MapToErrorDetails(FetchClaimsError fetchClaimsError)
    {
        return fetchClaimsError switch
        {
            FetchClaimsError.DiscordClaimsMissing => throw new NotImplementedException(),
            FetchClaimsError.GithubClaimsMissing => throw new NotImplementedException(),
            FetchClaimsError.TwitterClaimsMissing => throw new NotImplementedException(),
            FetchClaimsError.GoogleClaimsMissing => throw new NotImplementedException(),
            FetchClaimsError.UnsupportedSSOProvider => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(fetchClaimsError), fetchClaimsError, null)
        };
    }
}
