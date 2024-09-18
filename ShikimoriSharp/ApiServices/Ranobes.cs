using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;
using ShikimoriSharp.Settings;

namespace ShikimoriSharp.ApiServices;

public class Ranobes : ApiBase
{
    public Ranobes(ApiClient apiClient, ILogger<ApiBase> logger) : base(Version.v1, apiClient, logger)
    {
    }

    public async Task<Ranobe[]> GetRanobes(RanobeRequestSettings settings = null,
        AccessToken personalInformation = null)
    {
        return await RequestAsync<Ranobe[], RanobeRequestSettings>("ranobe", settings, personalInformation);
    }

    public async Task<MangaRanobeId?> GetRanobe(long id, AccessToken personalInformation = null)
    {
        return await RequestAsync<MangaRanobeId>($"ranobe/{id}", personalInformation);
    }
}