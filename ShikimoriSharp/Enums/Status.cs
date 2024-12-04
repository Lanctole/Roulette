using Newtonsoft.Json;
using System.ComponentModel;
namespace ShikimoriSharp.Enums;

public enum Status
{
    /// <summary>
    ///  Анонсировано
    /// </summary>
    [Description("Анонсировано")]
    [JsonProperty("anons")]
    anons,
    /// <summary>
    ///  Выходит
    /// </summary>
    [Description("Выходит")]
    [JsonProperty("ongoing")]
    ongoing,
    /// <summary>
    ///  Вышло
    /// </summary>
    [Description("Вышло")]
    [JsonProperty("released")]
    released
}