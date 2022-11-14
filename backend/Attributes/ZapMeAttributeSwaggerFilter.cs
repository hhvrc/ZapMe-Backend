using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ZapMe.Attributes;

public sealed class ZapMeAttributeSwaggerFilter : ISchemaFilter, IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        foreach (IOpenApiAttribute attribute in context.ParameterInfo?.GetCustomAttributes(true)?.OfType<IOpenApiAttribute>() ?? Enumerable.Empty<IOpenApiAttribute>())
        {
            attribute.Apply(parameter);
        }
        foreach (IOpenApiAttribute attribute in context.PropertyInfo?.GetCustomAttributes(true)?.OfType<IOpenApiAttribute>() ?? Enumerable.Empty<IOpenApiAttribute>())
        {
            attribute.Apply(parameter);
        }
    }
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        foreach (IOpenApiAttribute attribute in context.MemberInfo?.GetCustomAttributes(true)?.OfType<IOpenApiAttribute>() ?? Enumerable.Empty<IOpenApiAttribute>())
        {
            attribute.Apply(schema);
        }
    }
}
