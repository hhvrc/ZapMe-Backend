using System.ComponentModel.DataAnnotations;
using ZapMe.Constants;

namespace ZapMe.Attributes;

/// <summary>
/// Represents a validation attribute that can be applied to a password property or field.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class PasswordAttribute : ValidationAttribute
{
    /// <summary>
    /// The example password to use for the OpenApi schema.
    /// </summary>
    public const string ExamplePassword = "Hq2yP1B^Fho&zRHxHkEu";

    private const string _ErrMsgMustBeString = "Password must be a string";
    private static readonly string _ErrMsgTooShort = $"Password must be at least {GeneralHardLimits.PasswordMinLength} characters long";
    private static readonly string _ErrMsgTooLong = $"Password must be at most {GeneralHardLimits.PasswordMaxLength} characters long";

    /// <summary>
    /// Gets a value indicating whether the password should be validated.
    /// </summary>
    public bool ShouldValidate { get; }

    /// <summary>
    /// Initializes a new instance of the PasswordAttribute class.
    /// </summary>
    /// <param name="shouldValidate">A value indicating whether the password should be validated.</param>
    public PasswordAttribute(bool shouldValidate)
    {
        ShouldValidate = shouldValidate;
    }

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (!ShouldValidate) return ValidationResult.Success;

        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (value is not string username)
        {
            return new ValidationResult(_ErrMsgMustBeString);
        }

        if (username.Length < GeneralHardLimits.PasswordMinLength)
        {
            return new ValidationResult(_ErrMsgTooShort);
        }

        if (username.Length > GeneralHardLimits.PasswordMaxLength)
        {
            return new ValidationResult(_ErrMsgTooLong);
        }

        return ValidationResult.Success;
    }
}
