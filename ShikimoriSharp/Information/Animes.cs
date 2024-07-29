using System.Threading.Tasks;
using ShikimoriSharp.AdditionalRequests;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Settings;

namespace ShikimoriSharp.Information
{
    public class Animes : ApiBase
    {
        public Animes(ApiClient apiClient) : base(Version.v1, apiClient)
        {
        }

        public async Task<Anime[]> GetAnime(AnimeRequestSettings settings = null,
            AccessToken personalInformation = null)
        {
            return await Request<Anime[], AnimeRequestSettings>("animes", settings, personalInformation);
        }

        public async Task<AnimeRanobeId> GetAnime(long id, AccessToken personalInformation = null)
        {
            return await Request<AnimeRanobeId>($"animes/{id}", personalInformation);
        }

    }
}