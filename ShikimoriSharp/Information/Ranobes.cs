using Microsoft.Extensions.Logging;
using ShikimoriSharp.ApiServices;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;
using ShikimoriSharp.Settings;
using System.Threading.Tasks;

namespace ShikimoriSharp.Information
{
    public class Ranobes : ApiBase
    {
        public Ranobes(ApiClient apiClient) : base(Version.v1, apiClient)
        {
        }

        public async Task<Ranobe[]> GetRanobes(RanobeRequestSettings settings = null,
            AccessToken personalInformation = null)
        {
            return await RequestAsync<Ranobe[], RanobeRequestSettings>("ranobe", settings, personalInformation);
        }

        public async Task<MangaRanobeId> GetRanobe(long id, AccessToken personalInformation = null)
        {
            return await RequestAsync<MangaRanobeId>($"ranobe/{id}", personalInformation);
        }
    }
}