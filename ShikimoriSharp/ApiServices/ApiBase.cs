using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShikimoriSharp.Bases;
using Version = ShikimoriSharp.Enums.Version;

namespace ShikimoriSharp.ApiServices;

/// <summary>
///     Базовый класс  API Shikimori.
/// </summary>
public abstract class ApiBase
{
    private readonly ApiClient _apiClient;
    private readonly string _baseUrl;
    private readonly ILogger<ApiBase> _logger;

    protected ApiBase(Version version, ApiClient apiClient, ILogger<ApiBase> logger)
    {
        Version = version;
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <summary>
    ///     Версия API.
    /// </summary>
    public Version Version { get; }

    private string Site => new Uri(_apiClient._httpClient.BaseAddress, $"api/{GetApiVersionPath()}").ToString();

    private string GetApiVersionPath()
    {
        return Version switch
        {
            Version.v1 => "",
            _ => Version + "/"
        };
    }

    private static MultipartFormDataContent SerializeToHttpContent<T>(T obj)
    {
        if (obj is null) return null;
        var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        var content = new MultipartFormDataContent();
        foreach (var field in fields)
        {
            var value = field.GetValue(obj);
            if (value is null) continue;

            var stringContent = value switch
            {
                bool boolValue => new StringContent(boolValue ? "true" : "false"),
                _ => new StringContent(value.ToString())
            };
            content.Add(stringContent, field.Name);
        }

        return content;
    }

    /// <summary>
    ///     Выполняет запрос к API с параметрами.
    /// </summary>
    /// <typeparam name="TResult">Тип результата запроса.</typeparam>
    /// <typeparam name="TSettings">Тип настроек запроса.</typeparam>
    /// <param name="apiMethod">Метод API.</param>
    /// <param name="settings">Настройки запроса.</param>
    /// <param name="token">Токен доступа (необязательно).</param>
    /// <param name="method">HTTP метод (по умолчанию GET).</param>
    /// <returns>Результат запроса.</returns>
    public async Task<TResult> RequestAsync<TResult, TSettings>(string apiMethod, TSettings settings,
        AccessToken token = null, string method = "GET")
    {
        try
        {
            _logger.LogInformation($"Выполнение запроса: {apiMethod}, Метод: {method}");

            var settingsContent = SerializeToHttpContent(settings);
            var result = await _apiClient.RequestForm<TResult>($"{Site}{apiMethod}", settingsContent, token, method);

            _logger.LogInformation($"Запрос выполнен успешно: {apiMethod}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при выполнении запроса: {apiMethod}");
            throw new HttpRequestException();
        }
    }

    /// <summary>
    ///     Выполняет запрос к API без параметров.
    /// </summary>
    /// <typeparam name="TResult">Тип результата запроса.</typeparam>
    /// <param name="apiMethod">Метод API.</param>
    /// <param name="token">Токен доступа (необязательно).</param>
    /// <param name="method">HTTP метод (по умолчанию GET).</param>
    /// <returns>Результат запроса.</returns>
    public async Task<TResult> RequestAsync<TResult>(string apiMethod, AccessToken token = null, string method = "GET")
    {
        try
        {
            _logger.LogInformation($"Выполнение запроса: {apiMethod}, Метод: {method}");

            var result = await _apiClient.RequestForm<TResult>($"{Site}{apiMethod}", token);

            _logger.LogInformation($"Запрос выполнен успешно: {apiMethod}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при выполнении запроса: {apiMethod}");
            throw;
        }
    }
}