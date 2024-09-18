using Newtonsoft.Json;
using ShikimoriSharp.Bases;

namespace ShikimoriSharp.Classes;

public class Manga : AnimeMangaRanobeBase
{
    [JsonProperty("volumes")] public long Volumes { get; set; }
    [JsonProperty("chapters")] public long Chapters { get; set; }
}