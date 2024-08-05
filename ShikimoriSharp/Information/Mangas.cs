using ShikimoriSharp.Bases;
using ShikimoriSharp.Settings;
using System.Threading.Tasks;
using ShikimoriSharp.Classes;
using Microsoft.Extensions.Logging;

namespace ShikimoriSharp.Information
{
    public class Mangas : ApiBase
    {
        public Mangas(ApiClient apiClient) : base(Version.v1, apiClient)
        {
        }

        public async Task<Manga[]> GetMangas(MangaRequestSettings settings = null,
            AccessToken personalInformation = null)
        {
            return await RequestAsync<Manga[], MangaRequestSettings>("mangas", settings, personalInformation);
        }
    }
}