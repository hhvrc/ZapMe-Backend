using AngleSharp.Text;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using ZapMe.Constants;
using ZapMe.Utils;

namespace ZapMe.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class UsernameAttribute : ValidationAttribute, IParameterAttribute
{
    public const string UsernameRegex = /* lang=regex */ @"^[^\s].*[^\s]$";
    public const string ExampleUsername = "MyUsername";
    private const string _ErrMsgMustBeString = "Username must be a string";
    private static readonly string _ErrMsgTooShort = $"Username must be at least {GeneralHardLimits.UsernameMinLength} characters long";
    private static readonly string _ErrMsgTooLong = $"Username must be at most {GeneralHardLimits.UsernameMaxLength} characters long";
    private const string _ErrMsgNoWhiteSpaceAtEnds = "Username cannot start or end with whitespace";
    private const string _ErrMsgNoObnoxiousChars = "Username must not contain obnoxious characters";

    public bool ShouldValidate { get; }

    public UsernameAttribute(bool shouldValidate)
    {
        ShouldValidate = shouldValidate;
    }

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

        if (username[0].IsWhiteSpaceCharacter() || username[^1].IsWhiteSpaceCharacter())
        {
            return new ValidationResult(_ErrMsgNoWhiteSpaceAtEnds);
        }

        if (Verifiers.IsBadUiString(username))
        {
            return new ValidationResult(_ErrMsgNoObnoxiousChars);
        }

        return ValidationResult.Success;
    }

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
    public void Apply(OpenApiParameter parameter)
    {
        Apply(parameter.Schema);
    }
}
