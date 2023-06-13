namespace ZapMe.DTOs.SSO;

public readonly record struct ProviderDataDto(string ProviderName, string UserName, string Email, bool EmailVerified, DateTime ExpiresAtUtc);