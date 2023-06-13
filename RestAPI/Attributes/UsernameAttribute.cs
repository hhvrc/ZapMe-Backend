using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using ZapMe.Constants;
using ZapMe.Utils;

namespace ZapMe.Attributes;

/// <summary>
/// An attribute used to validate whether a username is valid.
/// </summary>
/// <remarks>
/// Inherits from <see cref="ValidationAttribute"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class UsernameAttribute : ValidationAttribute, IParameterAttribute
{
    /// <summary>
    /// Regular expression for username validation.
    /// </summary>
    public const string UsernameRegex = /* lang=regex */ @"^[^\s].*[^\s]$";

    /// <summary>
    /// Example username used to generate OpenApi documentation.
    /// </summary>
    public const string ExampleUsername = "MyUsername";

    private const string _ErrMsgMustBeString = "Username must be a string";
    private static readonly string _ErrMsgTooShort = $"Username must be at least {GeneralHardLimits.UsernameMinLength} characters long";
    private static readonly string _ErrMsgTooLong = $"Username must be at most {GeneralHardLimits.UsernameMaxLength} characters long";
    private const string _ErrMsgNoWhiteSpaceAtEnds = "Username cannot start or end with whitespace";
    private const string _ErrMsgNoObnoxiousChars = "Username must not contain obnoxious characters";
    private const string _ErrMsgCannotBeEmailAddress = "Username cannot be an email address";

    /// <summary>
    /// Indicates whether validation should be performed.
    /// </summary>
    public bool ShouldValidate { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsernameAttribute"/> class with the specified validation behavior.
    /// </summary>
    /// <param name="shouldValidate">True if validation should be performed; otherwise, false.</param>
    public UsernameAttribute(bool shouldValidate)
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

        if (username.Length < GeneralHardLimits.UsernameMinLength)
        {
            return new ValidationResult(_ErrMsgTooShort);
        }

        if (username.Length > GeneralHardLimits.UsernameMaxLength)
        {
            return new ValidationResult(_ErrMsgTooLong);
        }

        if (Char.IsWhiteSpace(username[0]) || Char.IsWhiteSpace(username[^1]))
        {
            return new ValidationResult(_ErrMsgNoWhiteSpaceAtEnds);
        }

        if (Verifiers.IsBadUiString(username))
        {
            return new ValidationResult(_ErrMsgNoObnoxiousChars);
        }

        if (EmailUtils.Parse(username).Success)
        {
            return new ValidationResult(_ErrMsgCannotBeEmailAddress);
        }

        return ValidationResult.Success;
    }

    /// <inheritdoc/>
    public void Apply(OpenApiSchema schema)
    {
        if (ShouldValidate)
        {
            schema.MinLength = GeneralHardLimits.UsernameMinLength;
            schema.MaxLength = GeneralHardLimits.UsernameMaxLength;
            schema.Pattern = UsernameRegex;
        }

        schema.Example = new OpenApiString(ExampleUsername);
    }

    /// <inheritdoc/>
    public void Apply(OpenApiParameter parameter)
    {
        Apply(parameter.Schema);
    }
}
