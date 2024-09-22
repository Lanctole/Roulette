using System.Text.Json;
using Roulette.Models.Shiki;
using ShikimoriSharp.Classes;

namespace Roulette.Services;

/// <summary>
///     Сервис для выполнения запросов к API и обработки ответов.
/// </summary>
public class ApiClientService
{
    private static readonly JsonSerializerOptions s_readOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiClientService> _logger;

    /// <summary>
    ///     Конструктор класса <see cref="ApiClientService" />.
    /// </summary>
    /// <param name="httpClientFactory">Фабрика для создания экземпляров <see cref="HttpClient" />.</param>
    /// <param name="logger">Логгер для записи информации о запросах и ошибках.</param>
    public ApiClientService(IHttpClientFactory httpClientFactory, ILogger<ApiClientService> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger;
    }

    /// <summary>
    ///     Выполняет GET-запрос к указанному конечному пункту и десериализует ответ.
    /// </summary>
    /// <typeparam name="T">Тип данных для десериализации.</typeparam>
    /// <param name="endpoint">URL конечного пункта запроса.</param>
    /// <returns>Объект типа <typeparamref name="T" />, полученный из ответа.</returns>
    /// <exception cref="Exception">Выбрасывается при ошибках запроса или десериализации.</exception>
    public async Task<T> GetAsync<T>(string endpoint)
    {
        try
        {
            _logger.LogInformation("Отправка GET-запроса к {Endpoint}.", endpoint);
            var httpClient = _httpClientFactory.CreateClient(nameof(ApiClientService));

            using (var response = await httpClient.GetAsync(endpoint))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Успешно получен ответ от {Endpoint}.", endpoint);
                return JsonSerializer.Deserialize<T>(content, s_readOptions);
            }
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e, "Ошибка при отправке запроса к {Endpoint}: {Message}", endpoint, e.Message);
            throw new HttpRequestException($"Ошибка при отправке запроса к {endpoint}: {e.Message}", e);
        }
        catch (JsonException e)
        {
            _logger.LogError(e, "Ошибка десериализации ответа от {Endpoint}: {Message}", endpoint, e.Message);
            throw new Exception($"Ошибка десериализации ответа от {endpoint}: {e.Message}", e);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Непредвиденная ошибка при запросе к {Endpoint}: {Message}", endpoint, e.Message);
            throw;
        }
    }

    /// <summary>
    ///     Получает список студий аниме.
    /// </summary>
    /// <returns>Список студий аниме.</returns>
    public async Task<List<Studio>> GetStudiosAsync()
    {
        return await GetAsync<List<Studio>>("anime/studios");
    }

    /// <summary>
    ///     Получает список жанров аниме.
    /// </summary>
    /// <returns>Список жанров аниме.</returns>
    public async Task<List<GenreModel>> GetGenresAsync()
    {
        return await GetAsync<List<GenreModel>>("anime/genres");
    }


    /// <summary>
    ///     Получает информацию о манге по указанному URL.
    /// </summary>
    /// <param name="url">URL манги.</param>
    /// <returns>Объект манги.</returns>
    public async Task<MangaRanobeId> GetMangaByIdAsync(string url) //-V3013
    {
        return await GetAsync<MangaRanobeId>(url);
    }

    /// <summary>
    ///     Получает информацию о ранобе по указанному URL.
    /// </summary>
    /// <param name="url">URL ранобе.</param>
    /// <returns>Объект ранобе.</returns>
    public async Task<MangaRanobeId> GetRanobeByIdAsync(string url)
    {
        return await GetAsync<MangaRanobeId>(url);
    }

    /// <summary>
    ///     Получает информацию о аниме по указанному URL.
    /// </summary>
    /// <param name="url">URL аниме.</param>
    /// <returns>Объект аниме.</returns>
    public async Task<AnimeId> GetAnimeByIdAsync(string url)
    {
        return await GetAsync<AnimeId>(url);
    }

    /// <summary>
    ///     Получает список издателей манги.
    /// </summary>
    /// <returns>Список издателей манги.</returns>
    public async Task<List<Publisher>> GetPublishersAsync()
    {
        return await GetAsync<List<Publisher>>("manga/publishers");
    }
}