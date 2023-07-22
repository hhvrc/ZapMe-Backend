using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using ZapMe.BusinessRules;

namespace ZapMe.Attributes;

/// <summary>
/// An attribute used to validate whether a display name is valid.
/// </summary>
/// <remarks>
/// Inherits from <see cref="ValidationAttribute"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class DisplaynameAttribute : ValidationAttribute, IParameterAttribute
{
    /// <summary>
    /// Regular expression for username validation.
    /// </summary>
    public const string DisplaynameRegex = /* lang=regex */ @"^[^\s].*[^\s]$";

    /// <summary>
    /// Example username used to generate OpenApi documentation.
    /// </summary>
    public const string ExampleDisplayname = "String";

    private const string _ErrMsgMustBeString = "Must be a string";

    /// <summary>
    /// Indicates whether validation should be performed.
    /// </summary>
    public bool ShouldValidate { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplaynameAttribute"/> class with the specified validation behavior.
    /// </summary>
    /// <param name="shouldValidate">True if validation should be performed; otherwise, false.</param>
    public DisplaynameAttribute(bool shouldValidate)
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

        if (value is not string displayname)
        {
            return new ValidationResult(_ErrMsgMustBeString);
        }

        if (Char.IsWhiteSpace(displayname[0]) || Char.IsWhiteSpace(displayname[^1]))
        {
            return new ValidationResult("Displayname cannot start or end with whitespace.");
        }

        if (UIStringValidator.IsBadUiString(displayname))
        {
            return new ValidationResult("Displayname must not contain obnoxious characters.");
        }

        return ValidationResult.Success;
    }

    /// <inheritdoc/>
    public void Apply(OpenApiSchema schema)
    {
        if (ShouldValidate)
        {
            schema.Pattern = DisplaynameRegex;
        }

        schema.Example = new OpenApiString(ExampleDisplayname);
    }

    /// <inheritdoc/>
    public void Apply(OpenApiParameter parameter)
    {
        Apply(parameter.Schema);
    }
}
