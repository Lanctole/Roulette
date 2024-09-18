using System.Threading.Tasks;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;

namespace ShikimoriSharp.ApiServices
{
    public class Publishers : ApiBase
    {
        public Publishers(ApiClient apiClient) : base(Version.v1, apiClient)
        {
        }

        public async Task<Publisher[]?> GetPublishers()
        {
            return await RequestAsync<Publisher[]>("publishers");
        }
    }
}