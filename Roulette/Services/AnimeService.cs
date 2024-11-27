using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Roulette.Models.Shiki;
using ShikimoriSharp.Classes;

namespace Roulette.Services;

public class AnimeService
{
    private readonly ApiConnectorService _apiConnectorService;
    private readonly IDistributedCache _cache;
    private readonly ILogger<AnimeService> _logger;

    public AnimeService(ApiConnectorService apiConnectorService, IDistributedCache cache, ILogger<AnimeService> logger)
    {
        _apiConnectorService = apiConnectorService;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<AnimeId>> GetAnimesAsync(string filterString)
    {
        var query = $@"
            {{
              animes({filterString}) {{
                episodes
                rating
                duration
                updatedAt
                studios {{
                  id
                  name
                }}
                english
                japanese
                synonyms
                licenseNameRu
                description
                descriptionHtml
                genres {{
                  id
                  name
                  russian
                }}
                kind
                score
                status
                airedOn {{
                    day
                    month
                    year
                }}
                id
                name
                russian
                poster{{
                    mainAltUrl
                }}
                url
              }}
            }}";

        var response = await _apiConnectorService.SendGraphQLQueryAsync(query);
        var animeResponse = JsonConvert.DeserializeObject<AnimeResponse>(response);
        return animeResponse.Animes;
    }
}

//public class AnimeResponseWrapper<T>
//{
//    public AnimeResponseData<T> Data { get; set; }
//}

public class AnimeResponse
{
    [JsonProperty("animes")]
    public List<AnimeId> Animes { get; set; }
}