using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Exceptions;
using ShikimoriSharp.Settings;

namespace ShikimoriSharp.ApiServices;

/// <summary>
///     Класс для управления запросами к API с учетом ограничения скорости и управления токенами доступа.
///     Обрабатывает отправку HTTP-запросов и управление токенами доступа.
/// </summary>
public class RequestManager
{
    private readonly HttpClient _httpClient;
    private readonly TokenBucket _bucketRpm;
    private readonly TokenBucket _bucketRps;
    private readonly ClientSettings _settings;
    private readonly Func<AccessToken, Task<AccessToken>> _refresh;
    private AccessToken _token;

    /// <summary>
    ///     Инициализирует новый экземпляр класса <see cref="RequestManager"/>.
    /// </summary>
    /// <param name="httpClient">Клиент HTTP для выполнения запросов.</param>
    /// <param name="bucketRps">Объект <see cref="TokenBucket"/> для управления запросами в секунду.</param>
    /// <param name="bucketRpm">Объект <see cref="TokenBucket"/> для управления запросами в минуту.</param>
    /// <param name="settings">Настройки клиента, включая имя клиента.</param>
    /// <param name="token">Токен доступа для аутентификации запросов.</param>
    /// <param name="refresh">Функция для обновления токена доступа.</param>
    public RequestManager(HttpClient httpClient,TokenBucket bucketRps, TokenBucket bucketRpm, ClientSettings settings, AccessToken token, Func<AccessToken, Task<AccessToken>> refresh)
    {
        _httpClient = httpClient;
        _bucketRps = bucketRps;
        _bucketRpm = bucketRpm;
        _settings = settings;
        _token = token;
        _refresh = refresh;
    }

    /// <summary>
    ///     Выполняет HTTP-запрос к указанному URI с использованием указанного метода и данных.
    /// </summary>
    /// <param name="dest">URI назначения для запроса.</param>
    /// <param name="method">HTTP метод (например, GET, POST).</param>
    /// <param name="data">Данные запроса в формате <see cref="HttpContent"/>.</param>
    /// <returns>Задача, представляющая результат запроса в виде <see cref="HttpResponseMessage"/>.</returns>
    /// <exception cref="Exception">Выбрасывается при ошибке обновления токена или ошибке выполнения запроса.</exception>
    private async Task<HttpResponseMessage> Response(string dest, string method, HttpContent data)
    {
        await _bucketRpm.TokenRequest();
        await _bucketRps.TokenRequest();

        var request = new HttpRequestMessage(new HttpMethod(method), dest)
        {
            Content = data
        };
        request.Headers.TryAddWithoutValidation("User-Agent", _settings.ClientName);

        if (_token != null)
        {
            request.Headers.TryAddWithoutValidation("Authorization", $"{_token.TokenType} {_token.Access_Token}");
        }

        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.BadRequest && response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        if (_refresh == null)
        {
            throw new Exception($"An error occurred while token refreshing: {response.StatusCode}");
        }

        _token = await _refresh(_token);
        throw new HttpRequestException($"{response.StatusCode}");

    }

    /// <summary>
    ///     Выполняет запрос и применяет политику повторных попыток в случае сбоя.
    /// </summary>
    /// <param name="dest">URI назначения для запроса.</param>
    /// <param name="method">HTTP метод (например, GET, POST).</param>
    /// <param name="data">Данные запроса в формате <see cref="HttpContent"/>.</param>
    /// <returns>Задача, представляющая результат запроса в виде строки.</returns>
    /// <exception cref="Exception">Выбрасывается при ошибке выполнения запроса или отсутствии ответа после повторных попыток.</exception>
    public async Task<string> ResponseExecutor(string dest, string method, HttpContent data)
    {
        var policy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)));

        HttpResponseMessage response;

        try
        {
            response = await policy.ExecuteAsync(() => Response(dest, method, data));
        }
        catch (Exception ex)
        {

            throw new Exception("An error occurred while response execution");
        }

        if (response == null)
        {
            throw new Exception("No response received after retries.");
        }

        switch (response.StatusCode)
        {
            case HttpStatusCode.UnprocessableEntity:
                throw new UnprocessableEntityException();
            case HttpStatusCode.Forbidden:
                throw new ForbiddenException();
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Unsuccessful request: {response.StatusCode} | {response.ReasonPhrase}");
        }

        var content = await response.Content.ReadAsStringAsync();
        return content;
    }

    /// <summary>
    ///     Выполняет запрос и десериализует ответ в указанный тип.
    /// </summary>
    /// <typeparam name="TResult">Тип, в который нужно десериализовать ответ.</typeparam>
    /// <param name="dest">URI назначения для запроса.</param>
    /// <param name="method">HTTP метод (например, GET, POST).</param>
    /// <param name="data">Данные запроса в формате <see cref="HttpContent"/>.</param>
    /// <returns>Задача, представляющая результат запроса в виде объекта типа <typeparamref name="TResult"/>.</returns>
    public async Task<TResult> ResponseAsType<TResult>(string dest, string method, HttpContent data)
    {
        var response = await ResponseExecutor(dest, method, data);
        return JsonConvert.DeserializeObject<TResult>(response);
    }
}