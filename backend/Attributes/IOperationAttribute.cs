using Microsoft.OpenApi.Models;

namespace ZapMe.Attributes;

public interface IOperationAttribute
{
    void Apply(OpenApiOperation operation);
}
