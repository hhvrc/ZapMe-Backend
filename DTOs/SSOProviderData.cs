namespace ZapMe.DTOs;

public sealed record SSOProviderData(string ProviderName, string ProviderUserId, string ProviderUserName, string ProviderUserEmail, bool ProviderUserEmailVerified, string? ProfilePictureUrl);
public sealed record SSOProviderDataEntry(string ProviderName, string ProviderUserId, string ProviderUserName, string ProviderUserEmail, bool ProviderUserEmailVerified, string? ProfilePictureUrl, DateTime ExpiresAt = default);