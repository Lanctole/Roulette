using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;

namespace ShikimoriSharp.Information
{
    public class Publishers : ApiBase
    {
        public Publishers(ApiClient apiClient) : base(Version.v1, apiClient)
        {
        }

        public async Task<Publisher[]> GetPublisher()
        {
            return await RequestAsync<Publisher[]>("publishers");
        }
    }
}