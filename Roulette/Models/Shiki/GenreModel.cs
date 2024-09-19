namespace Roulette.Models.Shiki;

/// <summary>
/// Модель для представления жанра получаемого от Shiki.
/// </summary>
public class GenreModel
{
    public List<int>? Ids { get; set; }
    public string? Name { get; set; }
    public string? Russian { get; set; }
    public string? Kind { get; set; }
}