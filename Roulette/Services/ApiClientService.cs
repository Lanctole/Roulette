using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShikimoriSharp.Classes;

using System.Text.Json;
using Roulette.Models.Shiki;

public class ApiClientService
{
    private readonly HttpClient _httpClient;

    public ApiClientService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    private static readonly JsonSerializerOptions s_readOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private async Task<T> GetAsync<T>(string endpoint)
    {
        try
        {
            Console.WriteLine("We in GetAsync");
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, s_readOptions);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"HttpRequestException in GetAsync: {e.Message}");
            throw new Exception($"Request to {endpoint} failed: {e.Message}", e);
        }
        catch (JsonException e)
        {
            Console.WriteLine($"JsonException in GetAsync: {e.Message}");
            throw new Exception($"Deserialization of response from {endpoint} failed: {e.Message}", e);
        }
    }

    public Task<List<Studio>> GetStudiosAsync()
    {
        return GetAsync<List<Studio>>("anime/studios");
    }

    public Task<List<GenreModel>> GetGenresAsync()
    {
        return GetAsync<List<GenreModel>>("anime/genres");
    } 

    public async Task<List<Anime>> GetAnimesAsync(string url)
    {
        Console.WriteLine("We in GetAnimesAsync");
        return await GetAsync<List<Anime>>(url);
    }

    public async Task<List<Manga>> GetMangasAsync(string url)
    {
        return await GetAsync<List<Manga>>(url);
    }

    public async Task<MangaRanobeId> GetMangaByIdAsync(string url) //-V3013
    {
        return await GetAsync<MangaRanobeId>(url);
    }

    public async Task<List<Ranobe>> GetRanobesAsync(string url)
    {
        return await GetAsync<List<Ranobe>>(url);
    }

    public async Task<MangaRanobeId> GetRanobeByIdAsync(string url)
    {
        return await GetAsync<MangaRanobeId>(url);
    }

    public async Task<AnimeId> GetAnimeByIdAsync(string url)
    {
        Console.WriteLine("public async Task<List<DetailedAnime>> GetAnimeByIdAsync(string url)");
        return await GetAsync<AnimeId>(url);
    }

    public async Task<List<Publisher>> GetPublishersAsync()
    {
        return await GetAsync<List<Publisher>>("manga/publishers");
    }
}
