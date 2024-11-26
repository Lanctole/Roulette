using Roulette.Models.Shiki;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Roulette.Services;

public class GenreService
{
    private readonly ApiConnectorService _apiConnectorService;
    private readonly IDistributedCache _cache;
    private readonly ILogger<GenreService> _logger;

    public GenreService(ApiConnectorService apiConnectorService, IDistributedCache cache, ILogger<GenreService> logger)
    {
        _apiConnectorService = apiConnectorService;
        _cache = cache;
        _logger = logger;
    }

    private async Task<GenresResponse> CheckCacheAsync(string cacheName)
    {
        string cachedData = null;
        try
        {
            cachedData = await _cache.GetStringAsync(cacheName);
        }
        catch (Exception cacheEx)
        {
            _logger.LogWarning(cacheEx, "Кэш недоступен, продолжаем без кэша для ключа {CacheKey}.", cacheName);
        }

        if (cachedData != null)
        {
            _logger.LogInformation("Получены данные из кэша по ключу {CacheKey}.", cacheName);
            return JsonConvert.DeserializeObject<GenresResponse>(cachedData);
        }

        return null;
    }

    private async Task SetCacheAsync(string cacheName, GenresResponse genresResponse)
    {
        try
        {
            string jsonData = JsonConvert.SerializeObject(genresResponse);
            await _cache.SetStringAsync(cacheName, jsonData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
            });
            _logger.LogInformation("Данные сохранены в кэш для ключа {CacheKey}.", cacheName);
        }
        catch (Exception cacheEx)
        {
            _logger.LogWarning(cacheEx, "Не удалось сохранить данные в кэш для ключа {CacheKey}.", cacheName);
        }
    }

    public async Task<List<GenreModel>> GetAnimeGenresAsync()
    {
        string cacheKey = "anime-genres";
        var cachedGenres = await CheckCacheAsync(cacheKey);

        if (cachedGenres != null)
        {
            return cachedGenres.Genres.OrderBy(genre => genre.Russian).ToList();
        }

        var query = @"
            {
              genres(entryType: Anime) {
                id
                name
                russian
                entryType
              }
            }";

        var response = await _apiConnectorService.SendGraphQLQueryAsync(query);
        var genresResponse = JsonConvert.DeserializeObject<GenresResponse>(response);

        var animeGenres = genresResponse?.Genres?.OrderBy(genre => genre.Russian).ToList() ?? new List<GenreModel>();
        await SetCacheAsync(cacheKey, genresResponse);
        return animeGenres;
    }

    public async Task<List<GenreModel>> GetMangaGenresAsync()
    {
        string cacheKey = "manga-genres";
        var cachedGenres = await CheckCacheAsync(cacheKey);

        if (cachedGenres != null)
        {
            return cachedGenres.Genres.OrderBy(genre => genre.Russian).ToList();
        }

        var query = @"
            {
              genres(entryType: Manga) {
                id
                name
                russian
                entryType
              }
            }";

        var response = await _apiConnectorService.SendGraphQLQueryAsync(query);
        var genresResponse = JsonConvert.DeserializeObject<GenresResponse>(response);

        var mangaGenres = genresResponse?.Genres?.OrderBy(genre => genre.Russian).ToList() ?? new List<GenreModel>();
        await SetCacheAsync(cacheKey, genresResponse);
        return mangaGenres;
    }
}

public class GenresResponse
{
    [JsonProperty("genres")]
    public List<GenreModel> Genres { get; set; }
}
