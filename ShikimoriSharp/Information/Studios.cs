using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShikimoriSharp.ApiServices;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;

namespace ShikimoriSharp.Information
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