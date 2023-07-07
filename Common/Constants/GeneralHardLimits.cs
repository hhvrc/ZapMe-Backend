namespace ZapMe.Constants;

public static class GeneralHardLimits
{
    public const int UsernameMinLength = 3;
    public const int UsernameMaxLength = 32;

    public const int EmailAddressMinLength = 5; // "a@b.c" (5 chars)
    public const int EmailAddressMaxLength = 320; // 64 + 1 + 255 (RFC 2821)

    public const int PasswordMinLength = 10;
    public const int PasswordMaxLength = 256;

    public const int UserAgentMaxLength = 2048;
    public const int UserAgentStoredLength = 512;

    public const int IPAddressMaxLength = 40;

    public const int StatusTextMaxLength = 128;
    public const int NickNameMaxLength = 32;
    public const int NotesMaxLength = 128;

    public static int UserDeviceNameMaxLength = 32;
}
