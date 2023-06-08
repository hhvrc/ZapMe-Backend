using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.SSO.Models;

public readonly struct SSOCreateAccount
{
    /// <summary/>
    public string SSOToken { get; init; }

    /// <summary/>
    public uint AcceptedPrivacyPolicyVersion { get; init; }

    /// <summary/>
    public uint AcceptedTermsOfServiceVersion { get; init; }

    /// <summary/>
    [Password(true)]
    public string Password { get; init; }
}
