using System.Text.Json;
using Roulette.Models.Films;

namespace Roulette.Services
{
    public class MovieApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MovieApiService> _logger;

        public MovieApiService(IHttpClientFactory httpClientFactory, ILogger<MovieApiService> logger)
        {
            var httpClientFactory1 = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _httpClient = httpClientFactory1.CreateClient(nameof(MovieApiService));
            _logger = logger;
        }

        private async Task<List<Genre>> GetValuesByFieldAsync(string field)
        {
            var response = await _httpClient.GetAsync($"{field}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Genre>>(content, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        public async Task<List<Genre>> GetGenresAsync()
        {
            return await GetValuesByFieldAsync("v1/movie/possible-values-by-field?field=genres.name");
        }

        public async Task<List<Genre>> GetCountriesAsync()
        {
            return await GetValuesByFieldAsync("v1/movie/possible-values-by-field?field=countries.name");
        }
        
        //public async Task<List<Movie>> GetMoviesAsync()
        //{
        //    return await GetValuesByFieldAsync("v1.4/movie/random");
        //}
    }
}
