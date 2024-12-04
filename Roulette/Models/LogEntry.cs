namespace Roulette.Models;

/// <summary>
/// Модель для хранения логов в БД
/// </summary>
public class LogEntry
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public LogLevel LogLevel { get; set; }
    public string Message { get; set; }
    public string Exception { get; set; }
    public string Logger { get; set; }
}