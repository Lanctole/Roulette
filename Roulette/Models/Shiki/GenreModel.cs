using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Roulette.Models.Shiki;

/// <summary>
/// Модель для представления жанра получаемого от Shiki.
/// </summary>
public class GenreModel
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("russian")]
    public string? Russian { get; set; }

    [JsonProperty("entryType")]
    public string? EntryType { get; set; }
}