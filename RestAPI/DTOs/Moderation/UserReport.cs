namespace ZapMe.DTOs.Moderation;

/// <summary>
/// Message sent to server to report a user
/// </summary>
public readonly struct UserReport
{
    public Guid UserId { get; init; }

    public string Title { get; init; }

    public string Explenation { get; init; }
}