namespace ZapMe.Records;

public enum UserOnlineStatus
{
    Offline,
    DoNotDisturb,
    Inactive,
    Online,
    DownBad
}

public interface IUserRecord
{
    /// <summary>
    /// 
    /// </summary>
    Guid Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string UserName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    Guid ProfilePictureId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    UserOnlineStatus OnlineStatus { get; set; }

    /// <summary>
    /// 
    /// </summary>
    string OnlineStatusText { get; set; }

    /// <summary>
    /// Date this account was created at
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last time this user was online
    /// </summary>
    DateTime LastOnline { get; set; }
}
