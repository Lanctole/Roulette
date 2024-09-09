using System.Threading.Tasks;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Settings;
using Version = ShikimoriSharp.Enums.Version;

namespace ShikimoriSharp.ApiServices
{
    public class Animes : ApiBase
    {
        public Animes(ApiClient apiClient) : base(Version.v1, apiClient)
        {
        }

        public async Task<Anime[]> GetAnimes(AnimeRequestSettings settings = null,
            AccessToken personalInformation = null)
        {
            return await RequestAsync<Anime[], AnimeRequestSettings>("animes", settings, personalInformation);
        }

        public async Task<AnimeId> GetAnime(long id, AccessToken personalInformation = null)
        {
            return await RequestAsync<AnimeId>($"animes/{id}", personalInformation);
        }

    }
}