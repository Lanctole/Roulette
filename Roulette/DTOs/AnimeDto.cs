namespace Roulette.DTOs;

/// <summary>
/// Модель данных для аниме. Избыточные свойства заключаются в поле Content
/// </summary>
public class AnimeDto
{
    public long Id { get; set; }
    public string Content { get; set; }
}