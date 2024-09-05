using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
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

        public ShikimoriApiConnectorService(IConfiguration configuration, IDistributedCache cache, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            var httpClient = _httpClientFactory.CreateClient(nameof(ShikimoriApiConnectorService));

            var authConfig = configuration.GetSection("Auth");
            var name = authConfig["Name"];
            var clientId = authConfig["ClientId"];
            var clientSecret = authConfig["ClientSecret"];

            _client = new ShikimoriClient(new ClientSettings(name, clientId, clientSecret), httpClient);
            _cache = cache;

            Console.WriteLine("ShikimoriApiConnectorService initialized");
        }

        private async Task<T> GetCachedOrApiData<T>(string cacheKey, Func<Task<T>> fetchDataFunc)
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (cachedData != null)
            {
                return JsonConvert.DeserializeObject<T>(cachedData);
            }

            var data = await fetchDataFunc();
            var serializedData = JsonConvert.SerializeObject(data);
            await _cache.SetStringAsync(cacheKey, serializedData);

            return data;
        }

        public Task<AnimeId> GetAnimeById(long id)
        {
            return GetCachedOrApiData($"Anime:{id}", () => _client.Animes.GetAnime(id));
        }

        public Task<MangaRanobeId> GetMangaById(long id)
        {
            return GetCachedOrApiData($"Manga:{id}", () => _client.Mangas.GetManga(id));
        }

        public Task<MangaRanobeId> GetRanobeById(long id)
        {
            return GetCachedOrApiData($"Ranobe:{id}", () => _client.Ranobes.GetRanobe(id));
        }

        public Task<Genre[]> GetGenres()
        {
            return GetCachedOrApiData("AnimeGenres_cache", () => _client.Genres.GetGenres());
        }

        public Task<Studio[]> GetStudios()
        {
            return GetCachedOrApiData("Studios_cache", () => _client.Studios.GetStudios());
        }

        public Task<Publisher[]> GetPublishers()
        {
            return GetCachedOrApiData("Publishers_cache", () => _client.Publishers.GetPublishers());
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
