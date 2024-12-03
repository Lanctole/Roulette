using System.Text.Json;
using Roulette.Models.Shiki;
using ShikimoriSharp.Classes;

namespace Roulette.Services;
//TODO : есть вероятность что можно безопасно удалить
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







}