using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Polly;
using Roulette.Data;
using Roulette.DTOs;
using ShikimoriSharp.Classes;

namespace Roulette.Services;

public class MangaService : ShikimoriBaseService
{
    private readonly ApiConnectorService _apiConnectorService;

    public MangaService(ApiConnectorService apiConnectorService, IDistributedCache cache, ApplicationDbContext context, ILogger<MangaService> logger) : base(cache, context, logger)
    {
        _apiConnectorService = apiConnectorService;
    }

    public async Task<List<MangaRanobeId>> GetMangasByIdsAsync(IEnumerable<long> ids)
    {
        return await GetOrFetchDataAsync(
            ids,
            async missingIds => await GetMangasAsync(CreateFilterString(missingIds)),
            async id => await _context.Mangas.AnyAsync(m => m.Id == id),
            async id =>
            {
                var cacheKey = $"Manga:{id}";
                var cachedData = await _cache.GetStringAsync(cacheKey);
                return cachedData != null ? JsonConvert.DeserializeObject<MangaRanobeId>(cachedData) : null;
            }
        );
    }

    public async Task<List<MangaRanobeId>> GetMangasAsync(string filterString)
    {
        var query = $@"
            {{
              mangas({filterString}) {{
                chapters
                publishers{{
                    id
                    name
                }}
                volumes
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
        var mangaResponse = JsonConvert.DeserializeObject<MangaResponse>(response);
        _ = CacheAndSaveToDBMangasAsync(mangaResponse.Mangas);
        return mangaResponse.Mangas;
    }

    private async Task CacheAndSaveToDBMangasAsync(List<MangaRanobeId> mangas)
    {
        try
        {
            foreach (var manga in mangas)
            {
                var cacheKey = $"Manga:{manga.Id}";

                var cachedData = await _cache.GetStringAsync(cacheKey);
                if (cachedData == null)
                {
                    var serializedData = JsonConvert.SerializeObject(manga);
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                    };
                    await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);
                }

                var existsInDb = await _context.Mangas.AnyAsync(m => m.Id == manga.Id);
                if (!existsInDb)
                {
                    await _context.Mangas.AddAsync(new MangaDto
                    {
                        Id = manga.Id,
                        Content = JsonConvert.SerializeObject(manga)
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка при обработке данных манги для кэша и базы данных.");
        }
    }

}

public class MangaResponse
{
    [JsonProperty("mangas")]
    public List<MangaRanobeId> Mangas { get; set; }
}