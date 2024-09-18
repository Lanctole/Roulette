using System;
using System.Net.Http;
using System.Threading.Tasks;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Settings;

namespace ShikimoriSharp.ApiServices;

/// <summary>
///     Класс для управления авторизацией и получения токенов доступа.
///     Обрабатывает запросы на получение и обновление токенов доступа через OAuth2.
/// </summary>
public class AuthorizationManager
{
    private readonly Func<string, HttpContent, Task<AccessToken>> _refreshFunc;
    private readonly ClientSettings _settings;
    private readonly string _tokenUrl;

    /// <summary>
    ///     Инициализирует новый экземпляр класса <see cref="AuthorizationManager" />.
    /// </summary>
    /// <param name="settings">Настройки клиента, содержащие идентификатор клиента и секрет.</param>
    /// <param name="refreshFunc">
    ///     Функция для выполнения запроса токена. Принимает URL и данные запроса, возвращает
    ///     <see cref="AccessToken" />.
    /// </param>
    /// <param name="baseUrl">Базовый URL для токенов доступа.</param>
    public AuthorizationManager(ClientSettings settings, Func<string, HttpContent, Task<AccessToken>> refreshFunc,
        Uri baseUrl)
    {
        _settings = settings;
        _refreshFunc = refreshFunc;
        _tokenUrl = new Uri(baseUrl, "oauth/token").ToString();
    }

    /// <summary>
    ///     Получает токен доступа используя код авторизации.
    /// </summary>
    /// <param name="authCode">Код авторизации, полученный после авторизации пользователя.</param>
    /// <returns>Задача, представляющая результат запроса токена доступа.</returns>
    public async Task<AccessToken> GetAccessToken(string authCode)
    {
        return await GetAccessTokenRequest(new MultipartFormDataContent
        {
            { new StringContent("authorization_code"), "grant_type" },
            { new StringContent(_settings.ClientId), "client_id" },
            { new StringContent(_settings.ClientSecret), "client_secret" },
            { new StringContent(authCode), "code" },
            { new StringContent(_settings.RedirectUrl), "redirect_uri" }
        });
    }

    /// <summary>
    ///     Обновляет токен доступа используя существующий токен обновления.
    /// </summary>
    /// <param name="oldToken">Старый токен, содержащий токен обновления.</param>
    /// <returns>Задача, представляющая результат запроса нового токена доступа.</returns>
    public async Task<AccessToken> RefreshAccessToken(AccessToken oldToken)
    {
        return await GetAccessTokenRequest(new MultipartFormDataContent
        {
            { new StringContent("refresh_token"), "grant_type" },
            { new StringContent(_settings.ClientId), "client_id" },
            { new StringContent(_settings.ClientSecret), "client_secret" },
            { new StringContent(oldToken.RefreshToken), "refresh_token" }
        });
    }

    /// <summary>
    ///     Выполняет запрос токена доступа используя указанные данные запроса.
    /// </summary>
    /// <param name="stringData">Данные запроса в формате <see cref="HttpContent" />.</param>
    /// <returns>Задача, представляющая результат запроса токена доступа.</returns>
    private async Task<AccessToken> GetAccessTokenRequest(HttpContent stringData)
    {
        return await _refreshFunc(_tokenUrl, stringData);
    }
}