using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct SocialEntry
{
    /// <summary>
    /// The name of the social platform
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Uri to the target page on the platform
    /// </summary>
    [JsonPropertyName("uri")]
    public Uri Uri { get; set; }

    /// <summary>
    /// The source from where to fetch the icon
    /// </summary>
    [JsonPropertyName("icon_type")]
    public IconSource IconSource { get; set; }

    /// <summary>
    /// The icon URI for dark theme, fetched from source defined by <see cref="IconSource"/>
    /// </summary>
    [JsonPropertyName("iconDark")]
    public string IconDarkURI { get; set; }

    /// <summary>
    /// The icon URI for light theme, fetched from source defined by <see cref="IconSource"/>
    /// </summary>
    [JsonPropertyName("iconLight")]
    public string IconLightURI { get; set; }
}