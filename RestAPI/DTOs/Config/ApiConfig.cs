namespace ZapMe.Controllers.Api.V1.Config.Models;

public readonly struct ApiConfig
{
    public ApiDisabledFeatures DisabledFeatures { get; init; }
}