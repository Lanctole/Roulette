using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Roulette.Data;
using Roulette.DTOs;

namespace Roulette.Services
{
    public class GameLanguageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;
        private const string LanguagesCacheKey = "GameLanguages_cache";

        public GameLanguageService(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<SupportedLanguageDto>> GetLanguagesAsync()
        {
            var cachedLanguages = await _cache.GetStringAsync(LanguagesCacheKey);

            if (cachedLanguages != null)
            {
                return JsonConvert.DeserializeObject<List<SupportedLanguageDto>>(cachedLanguages);
            }

            var languagesFromDb = await _context.SupportedLanguages
                .OrderBy(l => l.Russian)
                .Select(l => new SupportedLanguageDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Russian = l.Russian
                })
                .ToListAsync();

            var serializedLanguages = JsonConvert.SerializeObject(languagesFromDb);
            await _cache.SetStringAsync(LanguagesCacheKey, serializedLanguages);

            return languagesFromDb;
        }
    }
}
