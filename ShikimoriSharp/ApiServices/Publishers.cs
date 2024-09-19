using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;

namespace ShikimoriSharp.ApiServices;

public class Publishers : ApiBase
{
    public Publishers(ApiClient apiClient, ILogger<ApiBase> logger) : base(Version.v1, apiClient, logger)
    {
    }

    public async Task<Publisher[]?> GetPublishers()
    {
        return await RequestAsync<Publisher[]>("publishers");
    }
}