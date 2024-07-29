using System.Threading.Tasks;
using ShikimoriSharp.AdditionalRequests;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Settings;

namespace ShikimoriSharp.Bases
{
    public class MangaRanobeApiBase : ApiBase
    {
        private readonly string _query;

        public MangaRanobeApiBase(string query, ApiClient apiClient) : base(Version.v1, apiClient)
        {
            _query = query;
        }

        public async Task<Manga[]> GetBySearch(MangaRequestSettings settings = null,
            AccessToken personalInformation = null)
        {
            return await Request<Manga[], MangaRequestSettings>($"{_query}", settings, personalInformation);
        }

        public async Task<MangaRanobeId> GetById(long id, AccessToken personalInformation = null)
        {
            return await Request<MangaRanobeId>($"{_query}/{id}", personalInformation);
        }

    }
}