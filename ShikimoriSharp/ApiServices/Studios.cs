using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;

namespace ShikimoriSharp.ApiServices;

public class Studios : ApiBase
{
    public Studios(ApiClient apiClient, ILogger<ApiBase> logger) : base(Version.v1, apiClient, logger)
    {
    }

    public async Task<Studio[]?> GetStudios()
    {
        return await RequestAsync<Studio[]>("studios");
    }
}