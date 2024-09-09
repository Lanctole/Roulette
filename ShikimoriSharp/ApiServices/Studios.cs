using System.Threading.Tasks;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;

namespace ShikimoriSharp.ApiServices
{
    public class Studios : ApiBase
    {
        public Studios(ApiClient apiClient) : base(Version.v1, apiClient)
        {
        }

        public async Task<Studio[]> GetStudios()
        {
            return await RequestAsync<Studio[]>("studios");
        }
    }
}