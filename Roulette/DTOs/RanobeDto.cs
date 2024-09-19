namespace Roulette.DTOs;

/// <summary>
/// Модель данных для ранобе. Избыточные свойства заключаются в поле Content
/// </summary>
public class RanobeDto
{
    public long Id { get; set; }
    public string Content { get; set; }
}