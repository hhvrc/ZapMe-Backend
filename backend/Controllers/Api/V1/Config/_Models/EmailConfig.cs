using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct Emailconfig
{
    [JsonPropertyName("support")]
    public string EmailSupport { get; set; }

    [JsonPropertyName("contact")]
    public string EmailContact { get; set; }
}