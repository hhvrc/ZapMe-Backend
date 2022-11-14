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

public struct SignInResult
{
    public static SignInResult Success(SignInEntity signIn)
    {
        return new SignInResult(SignInResultType.Success, signIn);
    }


    public SignInResult(SignInResultType resultType, SignInEntity? signIn = null)
    {
        Result = resultType;
        SignIn = signIn;
    }

    public SignInResultType Result { get; }
    public SignInEntity? SignIn { get; }

    public static implicit operator SignInResult(SignInResultType resultType)
    {
        return new SignInResult(resultType);
    }
}

