using Games.Classes;
using Games.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Roulette.Data;
using Roulette.DTOs;

namespace Roulette.Services;

public class GameService
{
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GameService> _logger;

    public GameService(ApplicationDbContext context, IDistributedCache cache, ILogger<GameService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<long>> GetGameIdsAsync(IQueryable<Game> gameIdsQuery, int limit)
    {
        try
        {
            return await gameIdsQuery
                .Select(g => g.AppID)
                .Take(limit)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Ошибка при получении идентификаторов игр.", ex);
        }
    }

    public IQueryable<Game> ApplyFilters(
        string? genres,
        string? supportedLanguages,
        int? metacriticScoreMin,
        int? metacriticScoreMax,
        int? steamScoreMin,
        int? steamScoreMax,
        double? minCost,
        double? maxCost,
        string? releaseDateStart,
        string? releaseDateEnd)
    {
        var query = _context.Games.AsQueryable();
        if (!string.IsNullOrWhiteSpace(genres))
        {
            var genreIds = genres.Split(',').Select(g => int.Parse(g.Trim())).ToList();
            query = query.Where(g => g.Genres.Select(genre => genre.Id).Intersect(genreIds).Count() == genreIds.Count);
        }

        if (!string.IsNullOrWhiteSpace(supportedLanguages))
        {
            var languageIds = supportedLanguages.Split(',').Select(l => int.Parse(l.Trim())).ToList();
            query = query.Where(g =>
                g.SupportedLanguages.Select(lang => lang.Id).Intersect(languageIds).Count() == languageIds.Count);
        }

        if (metacriticScoreMin.HasValue || metacriticScoreMax.HasValue)
        {
            if (metacriticScoreMin.HasValue)
                query = query.Where(g => g.MetacriticScore >= metacriticScoreMin.Value);
            if (metacriticScoreMax.HasValue)
                query = query.Where(g => g.MetacriticScore <= metacriticScoreMax.Value);
        }

        if (steamScoreMin.HasValue || steamScoreMax.HasValue)
        {
            if (steamScoreMin.HasValue)
                query = query.Where(g => g.SteamScore >= steamScoreMin.Value);
            if (steamScoreMax.HasValue)
                query = query.Where(g => g.SteamScore <= steamScoreMax.Value);
        }

        if (minCost.HasValue && maxCost.HasValue && minCost == maxCost)
        {
            query = query.Where(g => g.Cost >= minCost.Value - 1 && g.Cost <= maxCost.Value);
        }
        else
        {
            if (minCost.HasValue)
                query = query.Where(g => g.Cost >= minCost.Value);

            if (maxCost.HasValue)
                query = query.Where(g => g.Cost <= maxCost.Value);
        }


        if (DateTime.TryParse(releaseDateStart, out var startDate))
        {
            startDate = startDate.ToUniversalTime();
            query = query.Where(g => g.ReleaseDate >= startDate);
        }

        if (DateTime.TryParse(releaseDateEnd, out var endDate))
        {
            endDate = endDate.ToUniversalTime();
            query = query.Where(g => g.ReleaseDate <= endDate);
        }

        return query;
    }

    public IQueryable<Game> ApplySorting(
        IQueryable<Game> query,
        GameOrder? order)
    {
        switch (order)
        {
            case GameOrder.Id:
                query = query.OrderBy(g => g.AppID);
                break;
            case GameOrder.SteamScore:
                query = query.OrderByDescending(g => g.SteamScore);
                break;
            case GameOrder.Name:
                query = query.OrderBy(g => g.Name);
                break;
            case GameOrder.ReleaseDate:
                query = query.OrderBy(g => g.ReleaseDate);
                break;
            case GameOrder.Random:
                query = query.OrderBy(g => EF.Functions.Random());
                break;
            default:
                query = query.OrderBy(g => g.Name);
                break;
        }

        return query;
    }

    public async Task<IEnumerable<GameDto>> GetGamesAsync(List<long> gameIds, int limit)
    {
        var games = new List<GameDto>();

        foreach (var gameId in gameIds.Take(limit))
        {
            var cacheKey = $"Game:{gameId}";
            GameDto gameFromDb = null;
            try
            {
                string cachedGame = null;
                try
                {
                    cachedGame = await _cache.GetStringAsync(cacheKey);
                }
                catch (Exception cacheEx)
                {
                    _logger.LogWarning(cacheEx, "Кэш недоступен, продолжаем без кэша для ID {GameId}.", gameId);
                }

                if (cachedGame != null)
                {
                    _logger.LogInformation("Получены данные игры из кэша для ID {GameId}.", gameId);
                    games.Add(JsonConvert.DeserializeObject<GameDto>(cachedGame));
                    continue;
                }

                try
                {
                    gameFromDb = await _context.Games
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
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Ошибка при получении игры из базы данных для ID {GameId}.", gameId);
                    throw new InvalidOperationException($"Не удалось получить данные игры для ID {gameId} из базы данных.", dbEx);
                }

                if (gameFromDb != null)
                {
                    var serializedGame = JsonConvert.SerializeObject(gameFromDb);
                    try
                    {
                        await _cache.SetStringAsync(cacheKey, serializedGame);
                    }
                    catch (Exception cacheEx)
                    {
                        _logger.LogWarning(cacheEx, "Ошибка при сохранении данных игры в кэш для ID {GameId}.", gameId);
                    }
                    _logger.LogInformation("Данные игры с ID {GameId} успешно получены из базы данных и сохранены в кэш.", gameId);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла непредвиденная ошибка при получении игры с ID {GameId}.", gameId);
                throw new InvalidOperationException($"Запрос не может быть обработан для игры с ID {gameId}", ex);
            }

            if (gameFromDb == null)
            {
                _logger.LogError($"Невозможно обработать запрос для игры с ID {gameId}: данные отсутствуют.");
                throw new InvalidOperationException($"Невозможно обработать запрос для игры с ID {gameId}: данные отсутствуют.");
            }
        }

        return games;
    }

}