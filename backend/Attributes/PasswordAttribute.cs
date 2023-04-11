using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;

namespace ZapMe.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class PasswordAttribute : ValidationAttribute, IParameterAttribute
{
    public const int MinPasswordLength = 10;
    public const int MaxPasswordLength = 256;
    public const string ExamplePassword = "Hq2yP1B^Fho&zRHxHkEu";
    private const string _ErrMsgMustBeString = "Password must be a string";
    private static readonly string _ErrMsgTooShort = $"Password must be at least {MinPasswordLength} characters long";
    private static readonly string _ErrMsgTooLong = $"Password must be at most {MaxPasswordLength} characters long";

    public bool ShouldValidate { get; }

    public PasswordAttribute(bool shouldValidate)
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

        if (username.Length < MinPasswordLength)
        {
            return new ValidationResult(_ErrMsgTooShort);
        }

        if (username.Length > MaxPasswordLength)
        {
            return new ValidationResult(_ErrMsgTooLong);
        }

        return ValidationResult.Success;
    }

    public void Apply(OpenApiSchema schema)
    {
        if (ShouldValidate)
        {
            schema.MinLength = MinPasswordLength;
            schema.MaxLength = MaxPasswordLength;
        }
        schema.Example = new OpenApiString(ExamplePassword);
    }
    public void Apply(OpenApiParameter parameter)
    {
        Apply(parameter.Schema);
    }
}
