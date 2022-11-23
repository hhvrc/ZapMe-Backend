using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace ZapMe.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class EmailAddressAttribute : ValidationAttribute, IParameterAttribute
{
    public const string ExampleEmail = "user.name@example.com";
    private const string _ErrMsgMustBeString = "Email address must be a string";
    private const string _ErrMsgInvalid = "Email address is invalid";

    public bool ShouldValidate { get; }

    public EmailAddressAttribute(bool shouldValidate)
    {
        ShouldValidate = shouldValidate;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (!ShouldValidate) return ValidationResult.Success;

        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not string email)
        {
            return new ValidationResult(_ErrMsgMustBeString);
        }

        if (!MailAddress.TryCreate(email, out _))
        {
            return new ValidationResult(_ErrMsgInvalid);
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
