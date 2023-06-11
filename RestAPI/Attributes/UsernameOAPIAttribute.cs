using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using ZapMe.Constants;

namespace ZapMe.Attributes;

public sealed class UsernameOAPIAttribute : UsernameAttribute, IParameterAttribute
{
    public UsernameOAPIAttribute(bool shouldValidate) : base(shouldValidate)
    {
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
