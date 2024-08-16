using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShikimoriSharp.ApiServices;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;

namespace ShikimoriSharp.Information
{
    public class Publishers : ApiBase
    {
        public Publishers(ApiClient apiClient) : base(Version.v1, apiClient)
        {
        }

        public async Task<Publisher[]> GetPublishers()
        {
            return await RequestAsync<Publisher[]>("publishers");
        }
    }
}