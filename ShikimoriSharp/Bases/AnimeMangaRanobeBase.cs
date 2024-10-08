﻿using System;
using Newtonsoft.Json;

namespace ShikimoriSharp.Bases;

public class AnimeMangaRanobeBase : SmallRepresentation
{
    [JsonProperty("kind")] public string Kind { get; set; }
    [JsonProperty("score")] public string Score { get; set; }
    [JsonProperty("status")] public string Status { get; set; }
    [JsonProperty("aired_on")] public DateTimeOffset? AiredOn { get; set; }
    [JsonProperty("released_on")] public DateTimeOffset? ReleasedOn { get; set; }
}