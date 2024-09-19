using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Roulette.Data;
using Roulette.DTOs;

namespace Roulette.Services;

/// <summary>
///     Сервис для работы с играми.
/// </summary>
public class GameService
{
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GameService> _logger;

    /// <summary>
    ///     Инициализирует новый экземпляр <see cref="GameService" />.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    /// <param name="cache">Кеш распределенной памяти.</param>
    /// <param name="logger">Логгер для записи информации и ошибок.</param>
    public GameService(ApplicationDbContext context, IDistributedCache cache, ILogger<GameService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    ///     Получает список игр по их идентификаторам.
    /// </summary>
    /// <param name="gameIds">Список идентификаторов игр.</param>
    /// <param name="limit">Максимальное количество игр для возвращения.</param>
    /// <returns>Список объектов <see cref="GameDto" />.</returns>
    public async Task<IEnumerable<GameDto>> GetGamesAsync(List<long> gameIds, int limit)
    {
        var games = new List<GameDto>();

        foreach (var gameId in gameIds.Take(limit))
        {
            var cacheKey = $"Game:{gameId}";
            try
            {
                var cachedGame = await _cache.GetStringAsync(cacheKey);

                if (cachedGame != null)
                {
                    _logger.LogInformation("Получены данные игры из кэша для ID {GameId}.", gameId);
                    games.Add(JsonConvert.DeserializeObject<GameDto>(cachedGame));
                    continue;
                }

                var gameFromDb = await _context.Games
                    .Include(g => g.Genres)
                    .Include(g => g.SupportedLanguages)
                    .Where(g => g.AppID == gameId)
                    .Select(g => new GameDto
                    {
                        AppID = g.AppID,
                        Name = g.Name,
                        Cost = g.Cost,
                        ReleaseDate = g.ReleaseDate,
                        ShortDescription = g.ShortDescription,
                        HeaderImage = g.HeaderImage,
                        SteamScore = g.SteamScore,
                        MetacriticScore = g.MetacriticScore,
                        Genres = g.Genres.Select(genre => genre.Name).ToList(),
                        SupportedLanguages = g.SupportedLanguages.Select(lang => lang.Name).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (gameFromDb != null)
                {
                    var serializedGame = JsonConvert.SerializeObject(gameFromDb);
                    await _cache.SetStringAsync(cacheKey, serializedGame);
                    _logger.LogInformation(
                        "Данные игры с ID {GameId} успешно получены из базы данных и сохранены в кэш.", gameId);
                    games.Add(gameFromDb);
                }
                else
                {
                    _logger.LogWarning("Игра с ID {GameId} не найдена в базе данных.", gameId);
                }
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Ошибка десериализации данных игры из кэша для ID {GameId}.", gameId);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Ошибка при работе с базой данных при получении игры с ID {GameId}.", gameId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла непредвиденная ошибка при получении игры с ID {GameId}.", gameId);
            }
        }

        return games;
    }
}