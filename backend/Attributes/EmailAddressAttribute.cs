using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using ZapMe.Utils;

namespace ZapMe.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class EmailAddressAttribute : ValidationAttribute, IParameterAttribute
{
    public const string ExampleEmail = "user.name@example.com";
    private const string _ErrMsgMustBeString = "Email address must be a string";
    private const string _ErrMsgInvalid = "Email address is invalid";
    private const string _ErrMsgDisplayNameNotAllowed = "Display name is not allowed in email address";
    private const string _ErrMsgAliasesNotAllowed = "Email address aliases are not allowed";

    public bool ShouldValidate { get; }
    public bool AllowDisplayName { get; }

    public EmailAddressAttribute(bool shouldValidate, bool allowDisplayName = false)
    {
        ShouldValidate = shouldValidate;
        AllowDisplayName = allowDisplayName;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (!ShouldValidate) return ValidationResult.Success;

        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (value is not string email)
        {
            return new ValidationResult(_ErrMsgMustBeString);
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
        if (ShouldValidate)
        {
        }
        schema.Format = "email";
        schema.Example = new OpenApiString(ExampleEmail);
    }
    public void Apply(OpenApiParameter parameter)
    {
        Apply(parameter.Schema);
    }
}
