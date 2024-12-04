using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Roulette.Data;
using Roulette.DTOs;
using ShikimoriSharp.Classes;

namespace Roulette.Services;

public class AnimeService : ShikimoriBaseService
{
    private readonly ApiConnectorService _apiConnectorService;

    public AnimeService(ApiConnectorService apiConnectorService, IDistributedCache cache, ApplicationDbContext context, ILogger<AnimeService> logger) : base(cache, context, logger)
    {
        _apiConnectorService = apiConnectorService;
    }

    public async Task<List<AnimeId>> GetAnimesByIdsAsync(IEnumerable<long> ids)
    {
        return await GetOrFetchDataAsync(
            ids,
            async missingIds => await GetAnimesAsync(CreateFilterString(missingIds)),
            async id => await _context.Animes.AnyAsync(a => a.Id == id),
            async id =>
            {
                var cacheKey = $"Anime:{id}";
                var cachedData = await _cache.GetStringAsync(cacheKey);
                return cachedData != null ? JsonConvert.DeserializeObject<AnimeId>(cachedData) : null;
            }
        );
    }

    public async Task<List<AnimeId>> GetAnimesAsync(string filterString)
    {
        var query = $@"
            {{
              animes({filterString}) {{
                episodes
                rating
                duration
                updatedAt
                studios {{
                  id
                  name
                }}
                english
                japanese
                synonyms
                licenseNameRu
                description
                descriptionHtml
                genres {{
                  id
                  name
                  russian
                }}
                kind
                score
                status
                airedOn {{
                    day
                    month
                    year
                }}
                id
                name
                russian
                poster{{
                    mainAltUrl
                }}
                url
              }}
            }}";

        var response = await _apiConnectorService.SendGraphQLQueryAsync(query);
        var animeResponse = JsonConvert.DeserializeObject<AnimeResponse>(response);
        _ = CacheAndSaveToDBAnimesAsync(animeResponse.Animes);
        return animeResponse.Animes;
    }

    private async Task CacheAndSaveToDBAnimesAsync(List<AnimeId> animes)
    {
        try
        {
            foreach (var anime in animes)
            {
                var cacheKey = $"Anime:{anime.Id}";

                var cachedData = await _cache.GetStringAsync(cacheKey);
                if (cachedData == null)
                {
                    var serializedData = JsonConvert.SerializeObject(anime);
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                    };
                    await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);
                }

                var existsInDb = await _context.Animes.AnyAsync(m => m.Id == anime.Id);
                if (!existsInDb)
                {
                    await _context.Animes.AddAsync(new AnimeDto
                    {
                        Id = anime.Id,
                        Content = JsonConvert.SerializeObject(anime)
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка при обработке данных аниме для кэша и базы данных.");
        }
    }
}

public class AnimeResponse
{
    [JsonProperty("animes")]
    public List<AnimeId> Animes { get; set; }
}