using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Roulette.Data;
using Roulette.DTOs;

namespace Roulette.Services
{
    public class GameService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public GameService(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<GameDto>> GetGamesAsync(List<long> gameIds, int limit)
        {
            var games = new List<GameDto>();
            foreach (var gameId in gameIds.Take(limit))
            {
                var cachedGame = await _cache.GetStringAsync($"Game:{gameId}");

                if (cachedGame != null)
                {
                    var game = JsonConvert.DeserializeObject<GameDto>(cachedGame);
                    games.Add(game);
                }
                else
                {
                    var gameFromDb = await _context.Games
                        .Include(g => g.Genres)
                        .Include(g => g.SupportedLanguages)
                        .FirstOrDefaultAsync(g => g.AppID == gameId);

                    if (gameFromDb != null)
                    {
                        GameDto gameDto = new GameDto
                        {
                            AppID = gameFromDb.AppID,
                            Name = gameFromDb.Name,
                            Cost = gameFromDb.Cost,
                            ReleaseDate = gameFromDb.ReleaseDate,
                            ShortDescription = gameFromDb.ShortDescription,
                            HeaderImage = gameFromDb.HeaderImage,
                            SteamScore = gameFromDb.SteamScore,
                            MetacriticScore = gameFromDb.MetacriticScore,
                            Genres = gameFromDb.Genres.Select(genre => genre.Name).ToList(),
                            SupportedLanguages = gameFromDb.SupportedLanguages.Select(lang => lang.Name).ToList()
                        };

                        var serializedGame = JsonConvert.SerializeObject(gameDto);
                        await _cache.SetStringAsync($"Game:{gameId}", serializedGame);

                        games.Add(gameDto);
                    }
                }
            }
            return games;
        }
    }
}
