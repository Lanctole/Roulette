using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Roulette.Data;
using Roulette.DTOs;

namespace Roulette.Services;

/// <summary>
///     Сервис для работы с жанрами игр.
/// </summary>
public class GameGenreService
{
    private const string GenresCacheKey = "GameGenres_cache";
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GameGenreService> _logger;

    /// <summary>
    ///     Инициализирует новый экземпляр <see cref="GameGenreService" />.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    /// <param name="cache">Интерфейс распределённого кэша.</param>
    /// <param name="logger">Логгер для записи информации и ошибок.</param>
    public GameGenreService(ApplicationDbContext context, IDistributedCache cache, ILogger<GameGenreService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    ///     Получает список жанров игр.
    /// </summary>
    /// <returns>Список жанров в виде <see cref="List{GenreDto}" />.</returns>
    public async Task<List<GenreDto>> GetGenresAsync()
    {
        try
        {
            var cachedGenres = await _cache.GetStringAsync(GenresCacheKey);

            if (cachedGenres != null)
                try
                {
                    _logger.LogInformation("Получены данные из кэша.");
                    return JsonConvert.DeserializeObject<List<GenreDto>>(cachedGenres);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Ошибка при десериализации данных из кэша.");
                }

            _logger.LogInformation("Данные не найдены в кэше. Запрашиваем из базы данных.");
            var genresFromDb = await _context.Genres
                .OrderBy(g => g.Russian)
                .Select(g => new GenreDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Russian = g.Russian
                })
                .ToListAsync();

            try
            {
                var serializedGenres = JsonConvert.SerializeObject(genresFromDb);
                await _cache.SetStringAsync(GenresCacheKey, serializedGenres);
                _logger.LogInformation("Данные успешно сохранены в кэш.");
            }
            catch (Exception cacheEx)
            {
                _logger.LogError(cacheEx, "Ошибка при записи данных в кэш.");
            }

            return genresFromDb;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении жанров.");
            throw;
        }
    }
}