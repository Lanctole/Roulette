using ShikimoriSharp.Classes;
using System.Text.Json;
using Roulette.Models.Shiki;

namespace Roulette.Services
{
    public class ApiClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        private static readonly JsonSerializerOptions s_readOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient(nameof(ApiClientService));

                using (var response = await httpClient.GetAsync(endpoint))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(content, s_readOptions);
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Request to {endpoint} failed: {e.Message}", e);
            }
            catch (JsonException e)
            {
                throw new Exception($"Deserialization of response from {endpoint} failed: {e.Message}", e);
            }
        }

        public async Task<List<Studio>> GetStudiosAsync()
        {
            return await GetAsync<List<Studio>>("anime/studios");
        }

        public async Task<List<GenreModel>> GetGenresAsync()
        {
            return await GetAsync<List<GenreModel>>("anime/genres");
        }


        public async Task<MangaRanobeId> GetMangaByIdAsync(string url) //-V3013
        {
            return await GetAsync<MangaRanobeId>(url);
        }

        public async Task<MangaRanobeId> GetRanobeByIdAsync(string url)
        {
            return await GetAsync<MangaRanobeId>(url);
        }

        public async Task<AnimeId> GetAnimeByIdAsync(string url)
        {
            return await GetAsync<AnimeId>(url);
        }

        public async Task<List<Publisher>> GetPublishersAsync()
        {
            return await GetAsync<List<Publisher>>("manga/publishers");
        }
    }
}