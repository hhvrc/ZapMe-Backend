using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using ZapMe.Constants;
using ZapMe.Utils;

namespace ZapMe.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class EmailAddressAttribute : ValidationAttribute, IParameterAttribute
{
    public const string ExampleEmail = "user.name@example.com";
    private const string _ErrMsgMustBeString = "Email address must be a string";
    private static readonly string _ErrMsgTooShort = $"Email address must be at least {GeneralHardLimits.EmailAddressMinLength} characters long";
    private static readonly string _ErrMsgTooLong = $"Email address must be at most {GeneralHardLimits.EmailAddressMaxLength} characters long";
    private const string _ErrMsgInvalid = "Email address is invalid";
    private const string _ErrMsgDisplayNameNotAllowed = "Display name is not allowed in email address";
    private const string _ErrMsgAliasesNotAllowed = "Email address aliases are not allowed";

    public bool AllowDisplayName { get; }

    public EmailAddressAttribute(bool allowDisplayName = false)
    {
        AllowDisplayName = allowDisplayName;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (value is not string email)
        {
            return new ValidationResult(_ErrMsgMustBeString);
        }

        if (email.Length < GeneralHardLimits.EmailAddressMinLength)
        {
            return new ValidationResult(_ErrMsgTooShort);
        }

        if (email.Length > GeneralHardLimits.EmailAddressMaxLength)
        {
            return new ValidationResult(_ErrMsgTooLong);
        }

        EmailUtils.ParsedResult parsed = EmailUtils.Parse(email);
        if (!parsed.Success)
        {
            return new ValidationResult(_ErrMsgInvalid);
        }

        if (parsed.HasDisplayName && !AllowDisplayName)
        {
            return new ValidationResult(_ErrMsgDisplayNameNotAllowed);
        }

        if (parsed.HasAlias)
        {
            return new ValidationResult(_ErrMsgAliasesNotAllowed);
        }

        return ValidationResult.Success;
    }

    public void Apply(OpenApiSchema schema)
    {
        schema.MinLength = GeneralHardLimits.EmailAddressMinLength;
        schema.MaxLength = GeneralHardLimits.EmailAddressMaxLength;
        schema.Format = "email";
        schema.Example = new OpenApiString(ExampleEmail);
    }
    public void Apply(OpenApiParameter parameter)
    {
        Apply(parameter.Schema);
    }
}
