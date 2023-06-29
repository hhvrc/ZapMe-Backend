namespace ZapMe.DTOs;

public readonly record struct UserCreationDto(string Username, string Email, bool EmailVerified, string Password, uint AcceptedPrivacyPolicyVersion, uint AcceptedTermsOfServiceVersion, Guid? ProfileAvatarImageId, Guid? ProfileBannerImageId);
