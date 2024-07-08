using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Settings;
using ShikimoriSharp;

namespace Roulette.Services
{
    public class ShikimoriApiConnector
    {
        private readonly ShikimoriClient _client;

        public ShikimoriApiConnector(IConfiguration configuration)
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
        }

        public Genre[] GetGenres()
        {
            Task<Genre[]> genres = _client.Genres.GetGenres();
            return genres.Result;
        }

        public Anime[] GetAnimes(AnimeRequestSettings settings)
        {
            Task<Anime[]> animes = _client.Animes.GetAnime(settings);
            return animes.Result;
        }

        public Studio[] GetStudios()
        {
            var studios = _client.Studios.GetStudios();
            return studios.Result;
        }
    }
}
