namespace ZapMe.DTOs;

public sealed record SSOProviderData(string ProviderName, string ProviderUserId, string ProviderUserName, string ProviderUserEmail, bool ProviderUserEmailVerified, string? ProviderAvatarUrl, string? ProviderBannerUrl);
public sealed record SSOProviderDataEntry(string ProviderName, string ProviderUserId, string ProviderUserName, string ProviderUserEmail, bool ProviderUserEmailVerified, string? ProviderAvatarUrl, string? ProviderBannerUrl, DateTime ExpiresAt = default);