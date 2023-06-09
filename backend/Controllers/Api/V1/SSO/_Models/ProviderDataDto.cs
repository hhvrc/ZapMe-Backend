namespace ZapMe.Controllers.Api.V1.SSO.Models;

public readonly record struct ProviderDataDto(string Name, string Email, bool EmailVerified);