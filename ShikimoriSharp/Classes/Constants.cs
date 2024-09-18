using Newtonsoft.Json;

namespace ShikimoriSharp.Classes;

public class ConstantsUserRate
{
    [JsonProperty("status")] public string[] Status { get; set; }
}

public class ConstantsAnimeManga : ConstantsUserRate
{
    [JsonProperty("kind")] public string[] Kind { get; set; }
}