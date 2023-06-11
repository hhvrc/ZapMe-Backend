using Microsoft.OpenApi.Models;

namespace ZapMe.Attributes;

/// <summary>
/// An attribute that specifies a binary payload for an operation in a Swagger/OpenAPI document.
/// This attribute should be applied to a method to describe the expected request body content type
/// and format. The attribute can be applied multiple times to a single method to specify multiple media types.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class BinaryPayloadOAPIAttribute : Attribute, IOperationAttribute
{
    private readonly bool _required;
    private readonly string[] _mediaTypes;

    /// <summary>
    /// Initializes a new instance of the BinaryPayloadAttribute class with the specified settings.
    /// </summary>
    /// <param name="Required">A boolean value indicating whether the payload is required.</param>
    /// <param name="MediaTypes">A variable-length list of strings representing the media types for the payload.</param>
    public BinaryPayloadOAPIAttribute(bool Required = true, params string[] MediaTypes)
    {
        _required = Required;
        _mediaTypes = MediaTypes;
    }

    /// <inheritdoc/>
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