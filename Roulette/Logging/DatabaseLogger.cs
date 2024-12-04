using Roulette.Data;
using Roulette.Models;

namespace Roulette.Logging;

/// <summary>
/// Реализует интерфейс <see cref="ILogger"/> и обеспечивает возможность логирования
/// записей в базу данных через контекст <see cref="ApplicationDbContext"/>.
/// </summary>
public class DatabaseLogger : ILogger
{
    private readonly ApplicationDbContext _context;
    private readonly string _categoryName;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DatabaseLogger"/>.
    /// </summary>
    /// <param name="context">Контекст базы данных для сохранения записей логирования.</param>
    /// <param name="categoryName">Имя категории логирования.</param>
    public DatabaseLogger(ApplicationDbContext context, string categoryName)
    {
        _context = context;
        _categoryName = categoryName;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    /// <summary>
    /// Запись логирования асинхронно.
    /// </summary>
    /// <typeparam name="TState">Тип состояния.</typeparam>
    /// <param name="logLevel">Уровень логирования.</param>
    /// <param name="eventId">Идентификатор события.</param>
    /// <param name="state">Состояние логирования.</param>
    /// <param name="exception">Исключение, связанное с логированием, если есть.</param>
    /// <param name="formatter">Функция форматирования сообщения логирования.</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;
        Task.Run(async () => await LogAsync(logLevel, state, exception, formatter));
    }

    private async Task LogAsync<TState>(LogLevel logLevel, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        var logEntry = new LogEntry
        {
            LogLevel = logLevel,
            Message = formatter(state, exception),
            Exception = exception?.ToString(),
            Logger = _categoryName,
            Timestamp = DateTime.UtcNow
        };

        await _context.Logs.AddAsync(logEntry);
        await _context.SaveChangesAsync();
    }
}