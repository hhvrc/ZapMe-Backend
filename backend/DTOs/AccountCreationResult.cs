using ZapMe.Data.Models;

namespace ZapMe.DTOs;

public readonly struct AccountCreationResult
{
    public AccountCreationResult(ResultE result, AccountEntity entity = null!, string? errorDetails = null)
    {
        Result = result;
        Entity = entity;
        ErrorDetails = errorDetails;
    }

    public enum ResultE
    {
        Success,
        NameAlreadyTaken,
        EmailAlreadyTaken,
        NameOrEmailInvalid,
        UnknownError,
    }
    public bool IsSuccess => Result == ResultE.Success;
    public ResultE Result { get; }
    public AccountEntity Entity { get; }
    public string? ErrorDetails { get; } // Exception.MessageText
}
