using Microsoft.OpenApi.Models;

namespace ZapMe.Attributes;

public interface IOpenApiAttribute
{
    void Apply(OpenApiSchema schema);
    void Apply(OpenApiParameter parameter);
}
