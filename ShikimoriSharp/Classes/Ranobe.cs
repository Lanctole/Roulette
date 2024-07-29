using Newtonsoft.Json;
using ShikimoriSharp.Bases;

namespace ShikimoriSharp.Classes
{
    public class Ranobe : AnimeMangaRanobeBase
    {
        [JsonProperty("volumes")] public long Volumes { get; set; }
        [JsonProperty("chapters")] public long Chapters { get; set; }
    }
}