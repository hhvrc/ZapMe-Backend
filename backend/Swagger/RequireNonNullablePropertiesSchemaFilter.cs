using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ZapMe.Swagger;

public class RequireNonNullablePropertiesSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Add to model.Required all properties where Nullable is false.
    /// </summary>
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        IEnumerable<string> additionalRequiredProps = model.Properties
            .Where(x => !x.Value.Nullable && !model.Required.Contains(x.Key))
            .Select(x => x.Key);

        foreach (string propKey in additionalRequiredProps)
        {
            model.Required.Add(propKey);
        }
    }
}