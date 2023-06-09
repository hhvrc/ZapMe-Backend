namespace ZapMe.DTOs;

public sealed record SSOStateData(string RedirectUrl);
public sealed record SSOStateDataEntry(string RedirectUrl, DateTime ExpiresAt = default);