using ZapMe.Data.Models;

namespace ZapMe.DTOs;

public enum SignInResultType
{
    Success,
    UserNotFound,
    LockedOut,
    PasswordInvalid,
    EmailNotConfirmed,
    InternalServerError
}

public readonly struct SignInResult
{
    public static SignInResult Success(SessionEntity session)
    {
        return new SignInResult { Result = SignInResultType.Success, SignIn = session };
    }

    public readonly SignInResultType Result { get; init; }
    public readonly SessionEntity? SignIn { get; init; }

    public static implicit operator SignInResult(SignInResultType resultType)
    {
        return new SignInResult { Result = resultType, SignIn = null };
    }
}

