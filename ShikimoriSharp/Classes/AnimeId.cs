using System;
using Newtonsoft.Json;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Enums;

namespace ShikimoriSharp.Classes;

public class AnimeId : AnimeMangaRanobeIdBase
{
    [JsonProperty("episodes")] public int Episodes { get; set; }
    //[JsonProperty("episodes_aired")] public long EpisodesAired { get; set; }
    [JsonProperty("rating")] public Rating Rating { get; set; }
    [JsonProperty("duration")] public Duration Duration { get; set; }
    [JsonProperty("updatedAt")] public DateTimeOffset? UpdatedAt { get; set; }
    [JsonProperty("nextEpisodeAt")] public DateTimeOffset? NextEpisodeAt { get; set; }
    [JsonProperty("studios")] public Studio[] Studios { get; set; }
}