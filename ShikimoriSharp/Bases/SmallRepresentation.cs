using Newtonsoft.Json;

namespace ShikimoriSharp.Bases;

public class SmallRepresentation
{
    [JsonProperty("id")] public long Id { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("russian")] public string Russian { get; set; }
    [JsonProperty("poster")] public Image Image { get; set; }
    [JsonProperty("url")] public string Url { get; set; }
}