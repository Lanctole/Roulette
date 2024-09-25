using Roulette.Data;

namespace Roulette.Services;

/// <summary>
///     Сервис для очистки старых записей логирования в фоновом режиме.
///     Удаляет записи, которые были созданы более одного месяца назад.
/// </summary>
public class LogCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public LogCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    ///     Выполняет асинхронную операцию очистки старых записей логирования.
    /// </summary>
    /// <param name="stoppingToken">Токен отмены для управления остановкой службы.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var monthAgo = DateTime.UtcNow.AddMonths(-1);

                var oldLogs = context.Logs.Where(log => log.Timestamp < monthAgo);
                context.Logs.RemoveRange(oldLogs);
                await context.SaveChangesAsync();
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}