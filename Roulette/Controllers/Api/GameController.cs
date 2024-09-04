using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Roulette.Data;
using Roulette.Models.Games;

namespace Roulette.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GameController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("games")]
        public async Task<IActionResult> GetGames(
            [FromQuery] string? genres = null,
            [FromQuery] int? metacriticScore = null,
            [FromQuery] int? steamScore = null,
            [FromQuery] string? releaseDateStart = null,
            [FromQuery] string? releaseDateEnd = null,
            [FromQuery] string? supportedLanguages = null,
            [FromQuery] double? minCost = null,
            [FromQuery] double? maxCost = null,
            [FromQuery] string? order = null,
            [FromQuery] int limit = 5)
        {
            var query = _context.Games
                .Include(g => g.Genres)
                .Include(g => g.SupportedLanguages)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(genres))
            {
                var genreIds = genres.Split(',').Select(g => int.Parse(g.Trim())).ToList();
                query = query
                    .Where(g => g.Genres.Any(genre => genreIds.Contains(genre.Id)));
            }
            if (!string.IsNullOrWhiteSpace(supportedLanguages))
            {
                var languageIds = supportedLanguages.Split(',').Select(l => int.Parse(l.Trim())).ToList();
                query = query
                    .Where(g => g.SupportedLanguages.Any(lang => languageIds.Contains(lang.Id)));
            }

            if (metacriticScore.HasValue)
            {
                query = query.Where(g => g.MetacriticScore >= metacriticScore.Value);
            }

            if (steamScore.HasValue)
            {
                query = query.Where(g => g.SteamScore >= steamScore.Value);
            }

            if (minCost.HasValue)
            {
                query = query.Where(g => g.Cost >= minCost.Value);
            }

            if (maxCost.HasValue)
            {
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

            if (order == "random")
            {
                query = query.OrderBy(g => EF.Functions.Random());
            }
            else
            {
                query = query.OrderBy(g => g.Name);
            }

            var games = await query.Take(limit).ToListAsync();

            var result = games.Select(g => new
            {
                g.AppID,
                g.Name,
                g.Cost,
                g.ReleaseDate,
                g.ShortDescription,
                g.HeaderImage,
                g.SteamScore,
                g.MetacriticScore,
                Genres = g.Genres.Select(genre => genre.Name).ToList(),
                SupportedLanguages = g.SupportedLanguages.Select(supportedLanguage => supportedLanguage.Name).ToList()
            });

            return Ok(result);
        }

        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await _context.Genres
                .OrderBy(g => g.Name)
                .Select(g => new { g.Id, g.Name })
                .ToListAsync();

            return Ok(genres);
        }

        [HttpGet("languages")]
        public async Task<IActionResult> GetLanguages()
        {
            var languages = await _context.SupportedLanguages
                .OrderBy(l => l.Name)
                .Select(l => new { l.Id, l.Name })
                .ToListAsync();

            return Ok(languages);
        }
    }
}

