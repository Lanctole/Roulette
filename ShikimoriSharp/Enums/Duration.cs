using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ShikimoriSharp.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Duration
{
    /// <summary>
    /// Duration is less than 10 minutes.
    /// </summary>
    [Description("Продолжительность более 10 минут.")]
    S,
    /// <summary>
    /// Duration is less than 30 minutes.
    /// </summary>
    [Description("Продолжительность менее 30 минут")]
    D,
    /// <summary>
    /// Duration is more than 30 minutes.
    /// </summary>
    [Description("Продолжительность более 30 минут")]
    F
}