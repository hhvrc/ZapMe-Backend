using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using ZapMe.Constants;

namespace ZapMe.Attributes;

public sealed class PasswordOAPIAttribute : PasswordAttribute, IParameterAttribute
{
    public PasswordOAPIAttribute(bool shouldValidate) : base(shouldValidate)
    {
    }

    /// <inheritdoc/>
    public void Apply(OpenApiSchema schema)
    {
        if (ShouldValidate)
        {
            schema.MinLength = GeneralHardLimits.PasswordMinLength;
            schema.MaxLength = GeneralHardLimits.PasswordMaxLength;
        }
        schema.Example = new OpenApiString(ExamplePassword);
    }

    /// <inheritdoc/>
    public void Apply(OpenApiParameter parameter)
    {
        Apply(parameter.Schema);
    }
}
