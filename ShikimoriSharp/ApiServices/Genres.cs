using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;

namespace ShikimoriSharp.ApiServices;

public class Genres : ApiBase
{
    public Genres(ApiClient apiClient, ILogger<ApiBase> logger) : base(Version.v1, apiClient, logger)
    {
    }

    public async Task<Genre[]?> GetGenres()
    {
        return await RequestAsync<Genre[]>("genres");
    }
}