using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Roulette.Components.Pages.Shikimori;
using Roulette.Data;
using Roulette.DTOs;
using ShikimoriSharp.ApiServices;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Settings;
using Anime = ShikimoriSharp.Classes.Anime;
using Manga = ShikimoriSharp.Classes.Manga;
using Ranobe = ShikimoriSharp.Classes.Ranobe;

namespace Roulette.Services
{
    public class ShikimoriApiConnectorService
    {
        private readonly ShikimoriClient _client;
        private readonly IDistributedCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApplicationDbContext _context;

        public ShikimoriApiConnectorService(IConfiguration configuration, IDistributedCache cache, IHttpClientFactory httpClientFactory, ApplicationDbContext context)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            var httpClient = _httpClientFactory.CreateClient(nameof(ShikimoriApiConnectorService));

            var authConfig = configuration.GetSection("Auth");
            var name = authConfig["Name"];
            var clientId = authConfig["ClientId"];
            var clientSecret = authConfig["ClientSecret"];

            _client = new ShikimoriClient(new ClientSettings(name, clientId, clientSecret), httpClient);
            _cache = cache;
            _context = context;

            Console.WriteLine("ShikimoriApiConnectorService initialized");
        }

        private async Task<T> GetDataAsync<T>(string cacheKey, Func<Task<T>> fetchFromApiFunc, Func<long, Task<T>> fetchFromDbFunc = null,
            Action<T> saveToDbAction = null, bool isForDb = false)
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (cachedData != null)
            {
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
                    return dbData;
                }
            }

            var apiData = await fetchFromApiFunc();
            var serializedApiData = JsonConvert.SerializeObject(apiData);

            if (isForDb && saveToDbAction != null)
            {
                saveToDbAction(apiData);
                await _context.SaveChangesAsync();
            }
            await _cache.SetStringAsync(cacheKey, serializedApiData);

            return apiData;
        }

        private long ExtractIdFromCacheKey(string cacheKey)
        {
            var parts = cacheKey.Split(':');
            return long.Parse(parts[1]);
        }

        public Task<AnimeId> GetAnimeById(long id)
        {
            return GetDataAsync(
                $"Anime:{id}",
                () => _client.Animes.GetAnime(id),
                async (id) =>
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
        public Task<MangaRanobeId> GetMangaById(long id)
        {
            return GetDataAsync(
                $"Manga:{id}",
                () => _client.Mangas.GetManga(id),
                async (id) =>
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

        public Task<MangaRanobeId> GetRanobeById(long id)
        {
            return GetDataAsync(
                $"Ranobe:{id}",
                () => _client.Ranobes.GetRanobe(id),
                async (id) =>
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
        
        public Task<Genre[]> GetGenres()
        {
            return GetDataAsync("AnimeGenres_cache", () => _client.Genres.GetGenres());
        }

        public Task<Studio[]> GetStudios()
        {
            return GetDataAsync("Studios_cache", () => _client.Studios.GetStudios());
        }

        public Task<Publisher[]> GetPublishers()
        {
            return GetDataAsync("Publishers_cache", () => _client.Publishers.GetPublishers());
        }

        public async Task<Anime[]> GetAnimes(AnimeRequestSettings settings)
        {
            return await _client.Animes.GetAnimes(settings);
        }

        public async Task<Manga[]> GetMangas(MangaRequestSettings settings)
        {
            return await _client.Mangas.GetMangas(settings);
        }

        public async Task<Ranobe[]> GetRanobes(RanobeRequestSettings settings)
        {
            return await _client.Ranobes.GetRanobes(settings);
        }
    }
}
