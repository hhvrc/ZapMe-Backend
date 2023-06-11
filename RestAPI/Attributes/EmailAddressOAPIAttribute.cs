using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using ZapMe.Constants;

namespace ZapMe.Attributes;

public sealed class EmailAddressOAPIAttribute : EmailAddressAttribute, IParameterAttribute
{
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
