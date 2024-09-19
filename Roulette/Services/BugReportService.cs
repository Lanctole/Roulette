using Microsoft.EntityFrameworkCore;
using Roulette.Data;
using Roulette.Models;

namespace Roulette.Services;

/// <summary>
///     Сервис для управления отчетами об ошибках.
/// </summary>
public class BugReportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BugReportService> _logger;

    /// <summary>
    ///     Инициализирует новый экземпляр <see cref="BugReportService" />.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    /// <param name="logger">Логгер для записи информации и ошибок.</param>
    public BugReportService(ApplicationDbContext context, ILogger<BugReportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    ///     Получает все отчеты об ошибках.
    /// </summary>
    /// <returns>Список всех отчетов об ошибках.</returns>
    public async Task<List<BugReport>> GetAllBugReportsAsync()
    {
        try
        {
            _logger.LogInformation("Получение всех отчетов об ошибках.");
            return await _context.BugReports.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении всех отчетов об ошибках.");
            throw;
        }
    }

    /// <summary>
    ///     Создает новый отчет об ошибке.
    /// </summary>
    /// <param name="bugReport">Отчет об ошибке.</param>
    /// <returns>Задача, представляющая асинхронную операцию создания отчета.</returns>
    public async Task CreateBugReportAsync(BugReport bugReport)
    {
        try
        {
            _context.BugReports.Add(bugReport);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Отчет об ошибке с ID {BugReportId} успешно создан.", bugReport.Id);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Ошибка при сохранении отчета об ошибке с ID {BugReportId}.", bugReport.Id);
            throw new InvalidOperationException("Не удалось создать отчет об ошибке.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла непредвиденная ошибка при создании отчета об ошибке.");
            throw;
        }
    }

    /// <summary>
    ///     Обновляет статус отчета об ошибке.
    /// </summary>
    /// <param name="id">Идентификатор отчета об ошибке.</param>
    /// <param name="newStatus">Новый статус отчета об ошибке.</param>
    /// <returns>Задача, представляющая асинхронную операцию обновления статуса.</returns>
    public async Task UpdateBugReportStatusAsync(int id, BugReportStatus newStatus)
    {
        try
        {
            var bugReport = await _context.BugReports.FindAsync(id);
            if (bugReport != null)
            {
                bugReport.Status = newStatus;
                bugReport.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Статус отчета об ошибке с ID {BugReportId} обновлен на {NewStatus}.", id,
                    newStatus);
            }
            else
            {
                _logger.LogWarning("Отчет об ошибке с ID {BugReportId} не найден.", id);
            }
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении статуса отчета об ошибке с ID {BugReportId}.", id);
            throw new InvalidOperationException($"Не удалось обновить статус отчета об ошибке с ID {id}.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла непредвиденная ошибка при обновлении статуса отчета об ошибке.");
            throw;
        }
    }
}