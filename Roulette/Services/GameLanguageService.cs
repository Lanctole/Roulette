using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Roulette.Data;
using Roulette.DTOs;

namespace Roulette.Services;

/// <summary>
///     Сервис для работы с языками, поддерживаемыми играми.
/// </summary>
public class GameLanguageService
{
    private const string LanguagesCacheKey = "GameLanguages_cache";
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GameLanguageService> _logger;

    /// <summary>
    ///     Инициализирует новый экземпляр <see cref="GameLanguageService" />.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    /// <param name="cache">Интерфейс распределённого кэша.</param>
    /// <param name="logger">Логгер для записи информации и ошибок.</param>
    public GameLanguageService(ApplicationDbContext context, IDistributedCache cache, ILogger<GameLanguageService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    ///     Получает список поддерживаемых языков.
    /// </summary>
    /// <returns>Список поддерживаемых языков в виде <see cref="List{SupportedLanguageDto}" />.</returns>
    public async Task<List<SupportedLanguageDto>> GetLanguagesAsync()
    {
        try
        {
            var cachedLanguages = await _cache.GetStringAsync(LanguagesCacheKey);

            if (cachedLanguages != null)
            {
                try
                {
                    _logger.LogInformation("Получены данные языков из кэша.");
                    return JsonConvert.DeserializeObject<List<SupportedLanguageDto>>(cachedLanguages);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Ошибка при десериализации данных языков из кэша.");
                }
            }

            _logger.LogInformation("Данные языков не найдены в кэше. Запрашиваем из базы данных.");
            var languagesFromDb = await _context.SupportedLanguages
                .OrderBy(l => l.Russian)
                .Select(l => new SupportedLanguageDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Russian = l.Russian
                })
                .ToListAsync();

            try
            {
                var serializedLanguages = JsonConvert.SerializeObject(languagesFromDb);
                await _cache.SetStringAsync(LanguagesCacheKey, serializedLanguages);
                _logger.LogInformation("Данные языков успешно сохранены в кэш.");
            }
            catch (Exception cacheEx)
            {
                _logger.LogError(cacheEx, "Ошибка при записи данных языков в кэш.");
            }

            return languagesFromDb;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка поддерживаемых языков.");
            throw;
        }
    }
}