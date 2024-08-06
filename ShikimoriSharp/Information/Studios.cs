using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;

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