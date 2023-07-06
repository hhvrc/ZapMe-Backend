using ZapMe.Constants;

namespace ZapMe.BusinessRules;

public static class UsernameValidator
{
    public readonly record struct ValidationResult(bool Ok, string ErrorMessage);

    public static ValidationResult Validate(string username)
    {
        if (username.Length < GeneralHardLimits.UsernameMinLength)
        {
            return new ValidationResult(false, "Username is too short.");
        }
        else if (username.Length > GeneralHardLimits.UsernameMaxLength)
        {
            return new ValidationResult(false, "Username is too long.");
        }

        if (Char.IsWhiteSpace(username[0]) || Char.IsWhiteSpace(username[^1]))
        {
            return new ValidationResult(false, "Username cannot start or end with whitespace.");
        }

        if (UIStringValidator.IsBadUiString(username))
        {
            return new ValidationResult(false, "Username must not contain obnoxious characters.");
        }

        if (EmailValidator.Parse(username).Success)
        {
            return new ValidationResult(false, "Username must not be an email address.");
        }

        return new ValidationResult(true, String.Empty);
    }
}
