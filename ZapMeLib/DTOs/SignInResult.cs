using ZapMe.Database.Models;

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

public struct SignInResult
{
    public static SignInResult Success(SessionEntity session)
    {
        return new SignInResult { Result = SignInResultType.Success, SignIn = session };
    }

    public SignInResultType Result { get; set; }
    public SessionEntity? SignIn { get; set; }

    public static implicit operator SignInResult(SignInResultType resultType)
    {
        return new SignInResult { Result = resultType, SignIn = null };
    }
}

