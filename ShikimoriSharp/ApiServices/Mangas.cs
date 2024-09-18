using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;
using ShikimoriSharp.Settings;

namespace ShikimoriSharp.ApiServices;

public class Mangas : ApiBase
{
    public Mangas(ApiClient apiClient, ILogger<ApiBase> logger) : base(Version.v1, apiClient, logger)
    {
    }

    public async Task<Manga[]> GetMangas(MangaRequestSettings settings = null,
        AccessToken personalInformation = null)
    {
        return await RequestAsync<Manga[], MangaRequestSettings>("mangas", settings, personalInformation);
    }

    public async Task<MangaRanobeId?> GetManga(long id, AccessToken personalInformation = null)
    {
        return await RequestAsync<MangaRanobeId>($"mangas/{id}", personalInformation);
    }
}