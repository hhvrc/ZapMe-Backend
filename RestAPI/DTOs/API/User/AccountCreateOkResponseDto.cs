namespace ZapMe.DTOs.API.User;

public readonly struct AccountCreateOkResponseDto
{
    /// <summary>
    /// The account id of the newly created account
    /// </summary>
    public Guid AccountId { get; init; }

    /// <summary>
    /// If the account was created using OAuth and in a way that ensures the email address is valid, a session will be created and returned here
    /// </summary>
    public AuthenticationResponse? Session { get; init; }

    /// <summary>
    /// If true then the email is already verified by 3rd party
    /// </summary>
    public bool EmailVerificationRequired { get; init; }
}
