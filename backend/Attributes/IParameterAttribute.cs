using Microsoft.OpenApi.Models;

namespace ZapMe.Attributes;

public interface IParameterAttribute
{
    void Apply(OpenApiSchema schema);
    void Apply(OpenApiParameter parameter);
}
