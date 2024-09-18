using System.Threading.Tasks;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;

namespace ShikimoriSharp.ApiServices
{
    public class Genres : ApiBase
    {
        public Genres(ApiClient apiClient) : base(Version.v1, apiClient)
        {
        }

        public async Task<Genre[]?> GetGenres()
        {
            return await RequestAsync<Genre[]>("genres");
        }
    }
}