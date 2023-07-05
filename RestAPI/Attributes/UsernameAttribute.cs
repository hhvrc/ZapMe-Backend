using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using ZapMe.BusinessRules;
using ZapMe.Constants;

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

        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not string username)
        {
            return new ValidationResult(_ErrMsgMustBeString);
        }

        var result = UsernameValidator.Validate(username);
        if (!result.Ok)
        {
            return new ValidationResult(result.ErrorMessage);
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
