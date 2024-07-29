using Newtonsoft.Json;
using ShikimoriSharp.Bases;

namespace ShikimoriSharp.Classes
{
    public class MangaRanobeId : AnimeMangaRanobeIdBase
    {
        [JsonProperty("volumes")] public long Volumes { get; set; }
        [JsonProperty("chapters")] public long Chapters { get; set; }
        [JsonProperty("publishers")] public Publisher[] Publishers { get; set; }
    }
}