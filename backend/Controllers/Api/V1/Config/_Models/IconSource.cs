namespace ZapMe.Controllers.Api.V1.Config.Models;

/// <summary>
/// The source of an icon, this is used to determine how to fetch the icon
/// - uri: The icon is a URI to an image
/// - emoji: The icon is an emoji
/// - google: The icon is a Google Material Design icon name (https://fonts.google.com/icons)
/// </summary>
public enum IconSource
{
    /// <summary>
    /// WWW Uri to the icon
    /// </summary>
    Uri,

    /// <summary>
    /// The icon text is the icon itself
    /// </summary>
    Emoji,

    /// <summary>
    /// <see href="https://fonts.google.com/icons">Google Material Design Icons</see> icon name
    /// </summary>
    GoogleMaterialDesign
}
