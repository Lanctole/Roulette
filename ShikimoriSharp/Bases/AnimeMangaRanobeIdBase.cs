using Newtonsoft.Json;
using ShikimoriSharp.Classes;

namespace ShikimoriSharp.Bases;

public class AnimeMangaRanobeIdBase : AnimeMangaRanobeBase
{
    [JsonProperty("english")] public string[] English { get; set; }

    [JsonProperty("japanese")] public string[] Japanese { get; set; }

    [JsonProperty("synonyms")] public string[] Synonyms { get; set; }

    [JsonProperty("license_name_ru")] public string LicenseNameRu { get; set; }

    [JsonProperty("description")] public string Description { get; set; }

    [JsonProperty("description_html")] public string DescriptionHtml { get; set; }

    [JsonProperty("genres")] public Genre[] Genres { get; set; }

}