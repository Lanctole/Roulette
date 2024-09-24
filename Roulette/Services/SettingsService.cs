using Roulette.Models;

namespace Roulette.Services;

/// <summary>
///     Сервис для получения настроек приложения.
/// </summary>
public class SettingsService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SettingsService> _logger;

    /// <summary>
    ///     Инициализирует новый экземпляр <see cref="SettingsService" />.
    /// </summary>
    /// <param name="configuration">Экземпляр конфигурации приложения.</param>
    /// <param name="logger">Логгер для записи информации и ошибок.</param>
    public SettingsService(IConfiguration configuration, ILogger<SettingsService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger;
    }

    /// <summary>
    ///     Получить адрес Shikimori.
    /// </summary>
    /// <returns>Возвращает экземпляр <see cref="BaseUrlSettings"/> с настройками базового URL.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public BaseUrlSettings GetShikimoriSettings()
    {
        try
        {
            var settings = _configuration.GetSection("Shikimori").Get<BaseUrlSettings>();

            if (settings == null)
            {
                _logger.LogError("Не удалось загрузить настройки Shikimori из конфигурации.");
                throw new InvalidOperationException("Не удалось загрузить настройки Shikimori.");
            }

            _logger.LogInformation("Настройки Shikimori успешно загружены.");
            return settings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при получении настроек Shikimori.");
            throw;
        }
    }

    /// <summary>
    ///     Получить адрес торрента
    /// </summary>
    /// <returns>Возвращает экземпляр <see cref="BaseUrlSettings"/> с настройками базового URL.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public BaseUrlSettings GetTorrentSettings()
    {
        try
        {
            var settings = _configuration.GetSection("Torrent").Get<BaseUrlSettings>();

            if (settings == null)
            {
                _logger.LogError("Не удалось загрузить настройки Torrent из конфигурации.");
                throw new InvalidOperationException("Не удалось загрузить настройки Torrent.");
            }

            _logger.LogInformation("Настройки Torrent успешно загружены.");
            return settings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при получении настроек Torrent.");
            throw;
        }
    }
}