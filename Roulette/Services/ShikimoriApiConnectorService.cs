using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Roulette.Data;
using Roulette.DTOs;
using ShikimoriSharp.ApiServices;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Settings;

namespace Roulette.Services;

//TODO разбить на отдельные сервисы
/// <summary>
///     Сервис для работы с API Shikimori и кэшем.
///     Обеспечивает получение данных о аниме, манге и ранобэ из API, базы данных или кэша.
/// </summary>
public class ShikimoriApiConnectorService
{
    private readonly IDistributedCache _cache;
    private readonly ShikimoriClient _client;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ShikimoriApiConnectorService> _logger;
    private readonly ILogger<ApiBase> _loggerForApi;

    /// <summary>
    ///     Конструктор сервиса ShikimoriApiConnectorService.
    /// </summary>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <param name="cache">Кэш для распределенных данных.</param>
    /// <param name="httpClientFactory">Фабрика HTTP-клиентов.</param>
    /// <param name="context">Контекст базы данных.</param>
    /// <param name="logger">Логгер для записи информации и ошибок.</param>
    public ShikimoriApiConnectorService(IConfiguration configuration, IDistributedCache cache,
        IHttpClientFactory httpClientFactory, ApplicationDbContext context, ILogger<ShikimoriApiConnectorService> logger, ILogger<ApiBase> loggerForApi)
    {
        var httpClientFactory1 = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        var httpClient = httpClientFactory1.CreateClient(nameof(ShikimoriApiConnectorService));

        var authConfig = configuration.GetSection("Auth");
        var name = Environment.GetEnvironmentVariable("Auth_Name") ?? configuration["Auth:Name"];
        var clientId = Environment.GetEnvironmentVariable("Auth_ClientId") ?? configuration["Auth:ClientId"];
        var clientSecret = Environment.GetEnvironmentVariable("Auth_ClientSecret") ?? configuration["Auth:ClientSecret"];


        _cache = cache;
        _context = context;
        _logger = logger;
        _loggerForApi = loggerForApi;
        if (name == null || clientId == null || clientSecret == null)
        {
            throw new ArgumentException("Не все параметры аутентификации заданы.");
        }

        _client = new ShikimoriClient(new ClientSettings(name, clientId, clientSecret), httpClient, loggerForApi);

        _logger.LogInformation("ShikimoriApiConnectorService initialized");

    }

    /// <summary>
    ///     Получает данные асинхронно из кэша, базы данных или API.
    /// </summary>
    /// <typeparam name="T">Тип данных.</typeparam>
    /// <param name="cacheKey">Ключ кэша для получения данных.</param>
    /// <param name="fetchFromApiFunc">Функция для получения данных из API.</param>
    /// <param name="fetchFromDbFunc">Функция для получения данных из базы данных.</param>
    /// <param name="saveToDbAction">Функция для сохранения данных в базу данных.</param>
    /// <param name="isForDb">Флаг указывающий, что данные должны быть сохранены в базу данных.</param>
    /// <returns>Возвращает данные типа T.</returns>
    private async Task<T?> GetDataAsync<T>(string cacheKey, Func<Task<T>> fetchFromApiFunc,
        Func<long, Task<T>> fetchFromDbFunc = null,
        Action<T> saveToDbAction = null, bool isForDb = false)
    {
        try
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (cachedData != null)
            {
                _logger.LogInformation("Получены данные из кэша по ключу {CacheKey}.", cacheKey);
                return JsonConvert.DeserializeObject<T>(cachedData);
            }

            var id = ExtractIdFromCacheKey(cacheKey);
            if (fetchFromDbFunc != null)
            {
                var dbData = await fetchFromDbFunc(id);
                if (dbData != null)
                {
                    var serializedData = JsonConvert.SerializeObject(dbData);
                    await _cache.SetStringAsync(cacheKey, serializedData);
                    _logger.LogInformation("Получены данные из базы данных и сохранены в кэш по ключу {CacheKey}.", cacheKey);
                    return dbData;
                }
            }

            var apiData = await fetchFromApiFunc();
            var serializedApiData = JsonConvert.SerializeObject(apiData);

            if (isForDb && saveToDbAction != null)
            {
                saveToDbAction(apiData);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Данные сохранены в базу данных и кэш по ключу {CacheKey}.", cacheKey);
            }

            await _cache.SetStringAsync(cacheKey, serializedApiData);
            _logger.LogInformation("Получены данные из API и сохранены в кэш по ключу {CacheKey}.", cacheKey);

            return apiData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении данных для ключа кэша {CacheKey}.", cacheKey);
            throw;
        }
    }

    /// <summary>
    ///     Извлекает идентификатор из ключа кэша.
    /// </summary>
    /// <param name="cacheKey">Ключ кэша.</param>
    /// <returns>Идентификатор в виде long.</returns>
    private long ExtractIdFromCacheKey(string cacheKey)
    {
        var parts = cacheKey.Split(':');
        return long.Parse(parts[1]);
    }

    /// <summary>
    ///     Получает аниме по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор аниме.</param>
    /// <returns>Объект AnimeId.</returns>
    public Task<AnimeId?> GetAnimeById(long id)
    {
        return GetDataAsync(
            $"Anime:{id}",
            () => _client.Animes.GetAnime(id),
            async id =>
            {
                var animeDto = await _context.Animes
                    .Where(a => a.Id == id)
                    .Select(a => new AnimeDto
                    {
                        Id = a.Id,
                        Content = a.Content
                    })
                    .FirstOrDefaultAsync();

                return animeDto != null
                    ? JsonConvert.DeserializeObject<AnimeId>(animeDto.Content)
                    : null;
            },
            anime =>
            {
                _context.Animes.Add(new AnimeDto
                {
                    Id = anime.Id,
                    Content = JsonConvert.SerializeObject(anime)
                });
            },
            true
        );
    }

    /// <summary>
    ///     Получает мангу по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор манги.</param>
    /// <returns>Объект MangaRanobeId.</returns>
    public Task<MangaRanobeId?> GetMangaById(long id)
    {
        return GetDataAsync(
            $"Manga:{id}",
            () => _client.Mangas.GetManga(id),
            async id =>
            {
                var mangaDto = await _context.Mangas
                    .Where(m => m.Id == id)
                    .Select(m => new MangaDto
                    {
                        Id = m.Id,
                        Content = m.Content
                    })
                    .FirstOrDefaultAsync();

                return mangaDto != null
                    ? JsonConvert.DeserializeObject<MangaRanobeId>(mangaDto.Content)
                    : null;
            },
            manga =>
            {
                _context.Mangas.Add(new MangaDto
                {
                    Id = manga.Id,
                    Content = JsonConvert.SerializeObject(manga)
                });
            },
            true
        );
    }

    /// <summary>
    ///     Получает ранобэ по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор ранобэ.</param>
    /// <returns>Объект MangaRanobeId.</returns>
    public Task<MangaRanobeId?> GetRanobeById(long id)
    {
        return GetDataAsync(
            $"Ranobe:{id}",
            () => _client.Ranobes.GetRanobe(id),
            async id =>
            {
                var ranobeDto = await _context.Ranobes
                    .Where(r => r.Id == id)
                    .Select(r => new RanobeDto
                    {
                        Id = r.Id,
                        Content = r.Content
                    })
                    .FirstOrDefaultAsync();

                return ranobeDto != null
                    ? JsonConvert.DeserializeObject<MangaRanobeId>(ranobeDto.Content)
                    : null;
            },
            ranobe =>
            {
                _context.Ranobes.Add(new RanobeDto
                {
                    Id = ranobe.Id,
                    Content = JsonConvert.SerializeObject(ranobe)
                });
            },
            true
        );
    }

    /// <summary>
    ///     Получает список жанров аниме.
    /// </summary>
    /// <returns>Массив объектов Genre.</returns>
    public Task<Genre[]?> GetGenres()
    {
        return GetDataAsync("AnimeGenres_cache", () => _client.Genres.GetGenres());
    }

    /// <summary>
    ///     Получает список студий.
    /// </summary>
    /// <returns>Массив объектов Studio.</returns>
    public Task<Studio[]?> GetStudios()
    {
        return GetDataAsync("Studios_cache", () => _client.Studios.GetStudios());
    }

    /// <summary>
    ///     Получает список издателей.
    /// </summary>
    /// <returns>Массив объектов Publisher.</returns>
    public Task<Publisher[]?> GetPublishers()
    {
        return GetDataAsync("Publishers_cache", () => _client.Publishers.GetPublishers());
    }

    /// <summary>
    ///     Получает список аниме по заданным параметрам.
    /// </summary>
    /// <param name="settings">Настройки запроса аниме.</param>
    /// <returns>Массив объектов Anime.</returns>
    public async Task<Anime[]> GetAnimes(AnimeRequestSettings settings)
    {
        return await _client.Animes.GetAnimes(settings);
    }

    /// <summary>
    ///     Получает список манги по заданным параметрам.
    /// </summary>
    /// <param name="settings">Настройки запроса манги.</param>
    /// <returns>Массив объектов Manga.</returns>
    public async Task<Manga[]> GetMangas(MangaRequestSettings settings)
    {
        return await _client.Mangas.GetMangas(settings);
    }

    /// <summary>
    ///     Получает список ранобэ по заданным параметрам.
    /// </summary>
    /// <param name="settings">Настройки запроса ранобэ.</param>
    /// <returns>Массив объектов Ranobe.</returns>
    public async Task<Ranobe[]> GetRanobes(RanobeRequestSettings settings)
    {
        return await _client.Ranobes.GetRanobes(settings);
    }
}