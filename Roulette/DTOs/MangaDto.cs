namespace Roulette.DTOs;

/// <summary>
///  Модель данных для манги. Избыточные свойства заключаются в поле Content
/// </summary>
public class MangaDto
{
    public long Id { get; set; }
    public string Content { get; set; }
}