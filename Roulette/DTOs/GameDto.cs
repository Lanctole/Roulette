namespace Roulette.DTOs;

/// <summary>
/// Модель данных для игр.
/// </summary>
public class GameDto
{
    public long AppID { get; set; }
    public string Name { get; set; }
    public double Cost { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string ShortDescription { get; set; }
    public string HeaderImage { get; set; }
    public int SteamScore { get; set; }
    public int MetacriticScore { get; set; }
    public List<string> Genres { get; set; } = new();
    public List<string> SupportedLanguages { get; set; } = new();
}