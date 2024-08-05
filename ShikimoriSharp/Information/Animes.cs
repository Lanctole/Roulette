using System;
using System.Threading.Tasks;
using ShikimoriSharp.AdditionalRequests;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Settings;
using Version = ShikimoriSharp.Bases.Version;

namespace ShikimoriSharp.Information
{
    public class Animes : ApiBase
    {
        public Animes(ApiClient apiClient) : base(Version.v1, apiClient)
        {
        }

        public async Task<Classes.Anime[]> GetAnime(AnimeRequestSettings settings = null,
            AccessToken personalInformation = null)
        {
            Console.WriteLine("We in ShikimoriSharp.Information");
            return await RequestAsync<Classes.Anime[], AnimeRequestSettings>("animes", settings, personalInformation);
        }

        public async Task<AnimeRanobeId> GetAnime(long id, AccessToken personalInformation = null)
        {
            return await RequestAsync<AnimeRanobeId>($"animes/{id}", personalInformation);
        }

    }
}