namespace ZapMe.Controllers.Api.V1.SSO.Models;

public readonly record struct ProviderDataDto(string ProviderName, string UserName, string Email, bool EmailVerified, DateTime ExpiresAtUtc);