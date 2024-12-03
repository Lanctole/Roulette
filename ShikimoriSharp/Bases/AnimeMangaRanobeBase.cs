using System;
using Newtonsoft.Json;
using ShikimoriSharp.Enums;

namespace ShikimoriSharp.Bases;

public class AnimeMangaRanobeBase : SmallRepresentation
{
    [JsonProperty("kind")] public string Kind { get; set; }
    [JsonProperty("score")] public string Score { get; set; }
    [JsonProperty("status")] public Status Status { get; set; }
    [JsonProperty("airedOn")] public AiredOn AiredOn { get; set; }
    [JsonProperty("releasedOn")] public DateTimeOffset? ReleasedOn { get; set; }
}

public class AiredOn
{
    [JsonProperty("day")] public int? Day { get; set; }
    [JsonProperty("month")] public int? Month { get; set; }
    [JsonProperty("year")] public int? Year { get; set; }

    public new string ToString()
    {
        return this.Day.ToString()+"." + this.Month.ToString() + "." + this.Year.ToString();
    }
}