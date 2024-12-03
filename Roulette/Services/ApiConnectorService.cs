using System.Text;
using System.Text.Json;

namespace Roulette.Services;

public class ApiConnectorService
{
    private readonly HttpClient _httpClient;

    public ApiConnectorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> SendGraphQLQueryAsync(string query)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(new { query }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync("http://shikimoriservice:8080/api/GraphQL/query", content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}