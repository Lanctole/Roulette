using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Roulette.Data;
using Roulette.DTOs;

namespace Roulette.Services
{
    public class GameGenreService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;
        private const string GenresCacheKey = "GenresCacheKey";

        public GameGenreService(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<GenreDto>> GetGenresAsync()
        {
            var cachedGenres = await _cache.GetStringAsync(GenresCacheKey);

            if (cachedGenres != null)
            {
                return JsonConvert.DeserializeObject<List<GenreDto>>(cachedGenres);
            }

            var genresFromDb = await _context.Genres
                .OrderBy(g => g.Name)
                .Select(g => new GenreDto
                {
                    Id = g.Id,
                    Name = g.Name
                })
                .ToListAsync();

            var serializedGenres = JsonConvert.SerializeObject(genresFromDb);
            await _cache.SetStringAsync(GenresCacheKey, serializedGenres);

            return genresFromDb;
        }
    }
}
