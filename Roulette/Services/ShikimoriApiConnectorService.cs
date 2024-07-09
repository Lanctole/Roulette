using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Settings;
using ShikimoriSharp;
using Microsoft.Extensions.Caching.Memory;

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

            long userIdLong = Convert.ToInt64(userId);
            var token = new AccessToken
            {
                Access_Token = access,
                RefreshToken = refresh,
                Scope = scope
            };

            _client = new ShikimoriClient(new ClientSettings(name, clientId, clientSecret));
            _cache = cache;
        }

        public Genre[] GetGenres()
        {
            return GetCachedData("genres_cache", () => _client.Genres.GetGenres());
        }

        public Anime[] GetAnimes(AnimeRequestSettings settings)
        {
            Task<Anime[]> animes = _client.Animes.GetAnime(settings);
            return animes.Result;
        }


        public Studio[] GetStudios()
        {
            return GetCachedData("studios_cache", () => _client.Studios.GetStudios());
        }

        private T GetCachedData<T>(string cacheKey, Func<Task<T>> fetchDataFunc)
        {
            if (!_cache.TryGetValue(cacheKey, out T cachedData))
            {
                Task<T> dataTask = fetchDataFunc();
                cachedData = dataTask.Result;

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                };

                _cache.Set(cacheKey, cachedData, cacheEntryOptions);
            }

            return cachedData;
        }
    }
}
