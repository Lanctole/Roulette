using Polly;
using Polly.RateLimit;
using Polly.Wrap;

namespace ShikimoriService;

public class GraphQLClient
{
    private readonly HttpClient _httpClient;
    private readonly AsyncPolicyWrap _policyWrap;

    public GraphQLClient(HttpClient httpClient)
    {
        _httpClient = httpClient;

        var perSecondPolicy = Policy.RateLimitAsync(5, TimeSpan.FromSeconds(1));
        var perMinutePolicy = Policy.RateLimitAsync(90, TimeSpan.FromMinutes(1));

        var retryPolicy = Policy
            .Handle<RateLimitRejectedException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        _policyWrap = Policy.WrapAsync(retryPolicy, perSecondPolicy, perMinutePolicy);
    }

    /// <summary>
    /// Отправка GraphQL-запроса.
    /// </summary>
    /// <typeparam name="TResult">Тип возвращаемого результата.</typeparam>
    /// <param name="query">Текст GraphQL-запроса.</param>
    /// <param name="variables">Переменные для GraphQL-запроса.</param>
    /// <returns>Результат выполнения запроса.</returns>
    public async Task<TResult?> ExecuteQueryAsync<TResult>(string query, object? variables = null)
    {
        return await _policyWrap.ExecuteAsync(async () =>
        {
            var request = new
            {
                query,
                variables
            };

            var response = await _httpClient.PostAsJsonAsync("/api/graphql", request);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"GraphQL запрос завершился с ошибкой: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }

            var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<TResult>>();
            if (result == null || result.Errors != null)
            {
                throw new ApplicationException($"Ошибка выполнения GraphQL-запроса: {string.Join(", ", result?.Errors.Select(e => e.Message) ?? new[] { "Неизвестная ошибка" })}");
            }

            return result.Data;
        });
    }
}

/// <summary>
/// Ответ GraphQL.
/// </summary>
/// <typeparam name="T">Тип данных в ответе.</typeparam>
public class GraphQLResponse<T>
{
    public T Data { get; set; }
    public GraphQLError[] Errors { get; set; }
}

/// <summary>
/// Ошибка GraphQL.
/// </summary>
public class GraphQLError
{
    public string Message { get; set; }
    public object Extensions { get; set; }
}