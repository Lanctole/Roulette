using Games.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Roulette.Data;
using Roulette.Services;

namespace Roulette.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly GameService _gameService;
        private readonly GameGenreService _genreService;
        private readonly GameLanguageService _languageService;

        public GameController(ApplicationDbContext context, GameService gameService, GameGenreService genreService, GameLanguageService languageService)
        {
            _context = context;
            _gameService = gameService;
            _genreService = genreService;
            _languageService = languageService;
        }

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
            gameIdsQuery = queryProcessor.ApplyFilters(gameIdsQuery, genres, supportedLanguages, metacriticScoreMin, metacriticScoreMax, 
                steamScoreMin, steamScoreMax, minCost, maxCost, releaseDateStart, releaseDateEnd);
            gameIdsQuery = queryProcessor.ApplySorting(gameIdsQuery, order);

            var gameIds = await gameIdsQuery.Select(g => g.AppID).Take(limit).ToListAsync();
            var games = await _gameService.GetGamesAsync(gameIds, limit);

            return Ok(games);
        }

        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await _genreService.GetGenresAsync();
            return Ok(genres);
        }

        [HttpGet("languages")]
        public async Task<IActionResult> GetLanguages()
        {
            var languages = await _languageService.GetLanguagesAsync();
            return Ok(languages);
        }
    }
}

