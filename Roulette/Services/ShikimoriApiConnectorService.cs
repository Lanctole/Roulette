using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ShikimoriSharp;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Settings;

namespace Roulette.Services
{
    public class ShikimoriApiConnectorService
    {
        private readonly ShikimoriClient _client;
        private readonly IMemoryCache _cache;

        public ShikimoriApiConnectorService(IConfiguration configuration, IMemoryCache cache)
        {
            var authConfig = configuration.GetSection("Auth");

            var scope = authConfig["Scope"];
            var access = authConfig["Access"];
            var refresh = authConfig["Refresh"];
            var name = authConfig["Name"];
            var clientId = authConfig["ClientId"];
            var clientSecret = authConfig["ClientSecret"];
            var userId = authConfig["UserId"];

            _client = new ShikimoriClient(new ClientSettings(name, clientId, clientSecret));
            _cache = cache;

            Console.WriteLine("ShikimoriApiConnectorService initialized");
        }

        public Task<Genre[]> GetGenres()
        {
            return GetCachedData("genres_cache", () => _client.Genres.GetGenres());
        }

        public async Task<Anime[]> GetAnimes(AnimeRequestSettings settings)
        {
            try
            {
                Console.WriteLine("Fetching animes with settings");
                return await _client.Animes.GetAnimes(settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting animes: {ex.Message}");
                throw;
            }
        }

        public async Task<AnimeId> GetAnimeById(long id)
        {
            try
            {
                return await _client.Animes.GetAnime(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting animes: {ex.Message}");
                throw;
            }
        }

        public Manga[] GetMangas(MangaRequestSettings settings)
        {
            try
            {
                Console.WriteLine("Fetching mangas with settings");
                Task<Manga[]> mangas = _client.Mangas.GetMangas(settings);
                return mangas.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting mangas: {ex.Message}");
                throw;
            }
        }

        public async Task<MangaRanobeId> GetMangaById(long id)
        {
            try
            {
                return await _client.Mangas.GetManga(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting manga: {ex.Message}");
                throw;
            }
        }

        public Ranobe[] GetRanobes(RanobeRequestSettings settings)
        {
            try
            {
                Console.WriteLine("Fetching ranobes with settings");
                Task<Ranobe[]> ranobes = _client.Ranobes.GetRanobes(settings);
                return ranobes.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting ranobes: {ex.Message}");
                throw;
            }
        }

        public async Task<MangaRanobeId> GetRanobeById(long id)
        {
            try
            {
                return await _client.Ranobes.GetRanobe(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting ranobe: {ex.Message}");
                throw;
            }
        }

        public Task<Studio[]> GetStudios()
        {
            return GetCachedData("studios_cache", () => _client.Studios.GetStudios());
        }
        
        public Task<Publisher[]> GetPublishers()
        {
            return GetCachedData("publishers_cache", () => _client.Publishers.GetPublishers());
        }

        private async Task<T> GetCachedData<T>(string cacheKey, Func<Task<T>> fetchDataFunc)
        {
            if (!_cache.TryGetValue(cacheKey, out T cachedData))
            {
                Console.WriteLine($"Cache miss for key {cacheKey}. Fetching data.");
                cachedData = await fetchDataFunc();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                };

                _cache.Set(cacheKey, cachedData, cacheEntryOptions);
                Console.WriteLine($"Cache updated for key {cacheKey}");
            }
            else
            {
                Console.WriteLine($"Cache hit for key {cacheKey}");
            }

            return cachedData;
        }
    }
}
