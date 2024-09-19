using Newtonsoft.Json;
using ShikimoriSharp.Bases;

namespace ShikimoriSharp.Classes;

public class Anime : AnimeMangaRanobeBase
{
    [JsonProperty("episodes")] public long Episodes { get; set; }
    [JsonProperty("episodes_aired")] public long EpisodesAired { get; set; }
}