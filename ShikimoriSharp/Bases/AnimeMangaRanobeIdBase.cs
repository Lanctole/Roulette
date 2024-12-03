using System.Linq;
using Newtonsoft.Json;
using ShikimoriSharp.Classes;

namespace ShikimoriSharp.Bases;

public class AnimeMangaRanobeIdBase : AnimeMangaRanobeBase
{
    [JsonProperty("english")] public string? English { get; set; }

    [JsonProperty("japanese")] public string? Japanese { get; set; }

    [JsonProperty("synonyms")] public string?[] Synonyms { get; set; }

    [JsonProperty("licenseNameRu")] public string? LicenseNameRu { get; set; }

    [JsonProperty("description")] public string? Description { get; set; }

    [JsonProperty("descriptionHtml")] public string? DescriptionHtml { get; set; }

    [JsonProperty("genres")] public Genre?[] Genres { get; set; }

    public string GetGenres()
    {
        return Genres != null
            ? string.Join(", ", Genres.Where(g => g != null).Select(g => g.Russian))
            : string.Empty;
    }
}