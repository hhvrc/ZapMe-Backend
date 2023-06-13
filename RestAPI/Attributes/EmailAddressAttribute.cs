using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using ZapMe.Constants;
using ZapMe.Utils;

namespace ZapMe.Attributes;

/// <summary>
/// Custom attribute used for validating email address properties or fields.
/// </summary>
/// <remarks>
/// This attribute implements the <see cref="ValidationAttribute"/> interface.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class EmailAddressAttribute : ValidationAttribute, IParameterAttribute
{
    /// <summary>
    /// The example email to use for the OpenApi schema.
    /// </summary>
    public const string ExampleEmail = "user.name@example.com";

    private const string _ErrMsgMustBeString = "Email address must be a string";
    private static readonly string _ErrMsgTooShort = $"Email address must be at least {GeneralHardLimits.EmailAddressMinLength} characters long";
    private static readonly string _ErrMsgTooLong = $"Email address must be at most {GeneralHardLimits.EmailAddressMaxLength} characters long";
    private const string _ErrMsgInvalid = "Email address is invalid";
    private const string _ErrMsgDisplayNameNotAllowed = "Display name is not allowed in email address";
    private const string _ErrMsgAliasesNotAllowed = "Email address aliases are not allowed";

    public bool AllowDisplayName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailAddressAttribute"/> class with the specified value indicating whether a display name is allowed in an email address.
    /// </summary>
    /// <param name="allowDisplayName">Indicates whether a display name is allowed in an email address.</param>
    public EmailAddressAttribute(bool allowDisplayName = false)
    {
        AllowDisplayName = allowDisplayName;
    }

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
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

    /// <inheritdoc/>
    public void Apply(OpenApiSchema schema)
    {
        schema.MinLength = GeneralHardLimits.EmailAddressMinLength;
        schema.MaxLength = GeneralHardLimits.EmailAddressMaxLength;
        schema.Format = "email";
        schema.Example = new OpenApiString(ExampleEmail);
    }

    /// <inheritdoc/>
    public void Apply(OpenApiParameter parameter)
    {
        Apply(parameter.Schema);
    }
}
