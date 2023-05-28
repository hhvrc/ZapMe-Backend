namespace ZapMe.Controllers.Api.V1.Authentication.OAuth.Models;

public readonly struct OAuthFinishAccountCreation
{
    /// <summary/>
    public string OAuthTicket { get; }

    /// <summary/>
    public uint AcceptedPrivacyPolicyVersion { get; init; }

    /// <summary/>
    public uint AcceptedTermsOfServiceVersion { get; init; }

    /// <summary/>
    public string Password { get; }
}
