using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes.Constants;

namespace ShikimoriSharp.Information
{
    public class Constants : ApiBase
    {
        public Constants(ApiClient apiClient) : base(Version.v1, apiClient)
        {
        }

        private async Task<ConstantsAnimeManga> Lesscode(string dest)
        {
            return await RequestAsync<ConstantsAnimeManga>(dest);
        }

        public async Task<ConstantsAnimeManga> GetAnimeConstants()
        {
            return await Lesscode("constants/anime");
        }

        public async Task<ConstantsAnimeManga> GetMangaConstants()
        {
            return await Lesscode("constants/manga");
        }

    }
}