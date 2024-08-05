using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShikimoriSharp.Classes
{
    public class DetailedAnime :Anime
    {
        [JsonProperty("rating")] public string Rating { get; set; }
        [JsonProperty("english")] public List<string> English { get; set; }
        [JsonProperty("japanese")] public List<string> Japanese { get; set; }
        [JsonProperty("synonyms")] public List<string> Synonyms { get; set; }
        [JsonProperty("duration")] public int Duration { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("description_html")] public string DescriptionHtml { get; set; }
        [JsonProperty("genres")] public List<Genre> Genres { get; set; }
        [JsonProperty("studios")] public List<Studio> Studios { get; set; }
    }
}
