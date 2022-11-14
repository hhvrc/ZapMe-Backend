using AngleSharp.Text;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using ZapMe.Logic;

namespace ZapMe.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class UsernameAttribute : ValidationAttribute, IOpenApiAttribute
{
    private const int MinLength = 3;
    private const int MaxLength = 32;
    private const string MsgMustBeString = "Username must be a string";
    private static readonly string MsgTooShort = $"Username must be at least {MinLength} characters long";
    private static readonly string MsgTooLong = $"Username must be at most {MaxLength} characters long";
    private const string MsgNoWhiteSpaceAtEnds = "Username cannot start or end with whitespace";
    private const string MsgNoObnoxiousChars = "Username must not contain obnoxious characters";

    public bool ShouldValidate { get; }

    public UsernameAttribute(bool shouldValidate)
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

        if (username[0].IsWhiteSpaceCharacter() || username[^1].IsWhiteSpaceCharacter())
        {
            return new ValidationResult(MsgNoWhiteSpaceAtEnds);
        }

        if (Verifiers.IsBadUiString(username))
        {
            return new ValidationResult(MsgNoObnoxiousChars);
        }

        return ValidationResult.Success;
    }

    public void Apply(OpenApiSchema schema)
    {
        if (ShouldValidate)
        {
            schema.MinLength = MinLength;
            schema.MaxLength = MaxLength;
            schema.Pattern = /* lang=regex */ @"^[^\s].*[^\s]$";
        }

        schema.Example = new OpenApiString("MyUsername");
    }
    public void Apply(OpenApiParameter parameter)
    {
        Apply(parameter.Schema);
    }
}
