using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;

namespace ZapMe.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class PasswordAttribute : ValidationAttribute, IOpenApiAttribute
{
    private const int MinLength = 10;
    private const int MaxLength = 256;
    private const string MsgMustBeString = "Password must be a string";
    private static readonly string MsgTooShort = $"Password must be at least {MinLength} characters long";
    private static readonly string MsgTooLong = $"Password must be at most {MaxLength} characters long";

    public bool ShouldValidate { get; }

    public PasswordAttribute(bool shouldValidate)
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

        if (value is not string username)
        {
            return new ValidationResult(MsgMustBeString);
        }

        if (username.Length < MinLength)
        {
            return new ValidationResult(MsgTooShort);
        }

        if (username.Length > MaxLength)
        {
            return new ValidationResult(MsgTooLong);
        }

        return ValidationResult.Success;
    }

    public void Apply(OpenApiSchema schema)
    {
        if (ShouldValidate)
        {
            schema.MinLength = MinLength;
            schema.MaxLength = MaxLength;
        }
        schema.Example = new OpenApiString("Hq2yP1B^Fho&zRHxHkEu");
    }
    public void Apply(OpenApiParameter parameter)
    {
        Apply(parameter.Schema);
    }
}
