using Microsoft.OpenApi.Models;

namespace ZapMe.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class BinaryPayloadAttribute : Attribute, IOperationAttribute
{
    private readonly bool _required;
    private readonly string[] _mediaTypes;

    public BinaryPayloadAttribute(bool Required = true, params string[] MediaTypes)
    {
        _required = Required;
        _mediaTypes = MediaTypes;
    }

    public void Apply(OpenApiOperation operation)
    {
        OpenApiRequestBody request = operation.RequestBody ??= new OpenApiRequestBody();
        request.Required = _required;

        IDictionary<string, OpenApiMediaType> content = request.Content ??= new Dictionary<string, OpenApiMediaType>();
        foreach (string mediaType in _mediaTypes)
        {
            content.Add(mediaType, new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary",
                    Description = "payload"
                }
            });
        }

        request.Required |= _required;
    }
}