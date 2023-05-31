using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.OAuth.Models;

public readonly struct OAuthFinishAccountCreation
{
    /// <summary/>
    public string OAuthTicket { get; init; }

    /// <summary/>
    public uint AcceptedPrivacyPolicyVersion { get; init; }

    /// <summary/>
    public uint AcceptedTermsOfServiceVersion { get; init; }

    /// <summary/>
    [Password(true)]
    public string Password { get; init; }
}
