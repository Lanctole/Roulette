using Games.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Roulette.Data;
using Roulette.Services;

namespace Roulette.Controllers.Api;

/// <summary>
///     Контроллер для управления играми.
/// </summary>
/// <remarks>
///     Этот контроллер предоставляет методы для получения списка игр, жанров и поддерживаемых языков.
///     Методы поддерживают фильтрацию и сортировку по различным параметрам.
/// </remarks>
[Route("api/[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly GameService _gameService;
    private readonly GameGenreService _genreService;
    private readonly GameLanguageService _languageService;

    /// <summary>
    ///     Конструктор контроллера.
    /// </summary>
    /// <param name="context">Контекст базы данных для доступа к информации об играх.</param>
    /// <param name="gameService">Сервис для работы с играми.</param>
    /// <param name="genreService">Сервис для работы с жанрами игр.</param>
    /// <param name="languageService">Сервис для работы с языками.</param>
    public GameController(ApplicationDbContext context, GameService gameService, GameGenreService genreService,
        GameLanguageService languageService)
    {
        _context = context;
        _gameService = gameService;
        _genreService = genreService;
        _languageService = languageService;
    }

    /// <summary>
    ///     Получает список игр на основе фильтров и параметров сортировки.
    /// </summary>
    /// <param name="genres">Список жанров, по которым нужно фильтровать игры (через запятую).</param>
    /// <param name="metacriticScoreMin">Минимальный рейтинг Metacritic.</param>
    /// <param name="metacriticScoreMax">Максимальный рейтинг Metacritic.</param>
    /// <param name="steamScoreMin">Минимальный рейтинг Steam.</param>
    /// <param name="steamScoreMax">Максимальный рейтинг Steam.</param>
    /// <param name="releaseDateStart">Дата начала релиза в формате YYYY-MM-DD.</param>
    /// <param name="releaseDateEnd">Дата окончания релиза в формате YYYY-MM-DD.</param>
    /// <param name="supportedLanguages">Список поддерживаемых языков (через запятую).</param>
    /// <param name="minCost">Минимальная стоимость игры.</param>
    /// <param name="maxCost">Максимальная стоимость игры.</param>
    /// <param name="order">Параметр для сортировки списка игр.</param>
    /// <param name="limit">Максимальное количество возвращаемых результатов.</param>
    /// <returns>Список игр, соответствующих указанным фильтрам и сортировке.</returns>
    [HttpGet("games")]
    public async Task<IActionResult> GetGames(
        [FromQuery] string? genres = null,
        [FromQuery] int? metacriticScoreMin = null,
        [FromQuery] int? metacriticScoreMax = null,
        [FromQuery] int? steamScoreMin = null,
        [FromQuery] int? steamScoreMax = null,
        [FromQuery] string? releaseDateStart = null,
        [FromQuery] string? releaseDateEnd = null,
        [FromQuery] string? supportedLanguages = null,
        [FromQuery] double? minCost = null,
        [FromQuery] double? maxCost = null,
        [FromQuery] GameOrder? order = null,
        [FromQuery] int limit = 5)
    {
        var gameIdsQuery = _context.Games.AsQueryable();
        var queryProcessor = new GameQueryProcessor();
        gameIdsQuery = queryProcessor.ApplyFilters(gameIdsQuery, genres, supportedLanguages, metacriticScoreMin,
            metacriticScoreMax,
            steamScoreMin, steamScoreMax, minCost, maxCost, releaseDateStart, releaseDateEnd);
        gameIdsQuery = queryProcessor.ApplySorting(gameIdsQuery, order);

        var gameIds = await gameIdsQuery.Select(g => g.AppID).Take(limit).ToListAsync();
        
        try
        {
            var games = await _gameService.GetGamesAsync(gameIds, limit);
            return Ok(games);
        }
        catch (InvalidOperationException operationExceptionEx)
        {
            return StatusCode(503, $"Ошибка запроса к API: {operationExceptionEx.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
        }
    }

    /// <summary>
    ///     Возвращает список всех доступных жанров игр.
    /// </summary>
    /// <returns>Список жанров игр.</returns>
    [HttpGet("genres")]
    public async Task<IActionResult> GetGenres()
    {
        var genres = await _genreService.GetGenresAsync();
        return Ok(genres);
    }

    /// <summary>
    ///     Возвращает список всех поддерживаемых языков.
    /// </summary>
    /// <returns>Список поддерживаемых языков.</returns>
    [HttpGet("languages")]
    public async Task<IActionResult> GetLanguages()
    {
        var languages = await _languageService.GetLanguagesAsync();
        return Ok(languages);
    }
}