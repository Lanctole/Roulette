using Microsoft.AspNetCore.Mvc;

namespace ShikimoriService;

/// <summary>
/// Контроллер для тестирования GraphQL-запросов.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GraphQLController : ControllerBase
{
    private readonly GraphQLClient _graphQLClient;

    public GraphQLController(GraphQLClient graphQLClient)
    {
        _graphQLClient = graphQLClient;
    }

    /// <summary>
    /// Тестовый запрос к GraphQL API.
    /// </summary>
    /// <param name="query">GraphQL-запрос.</param>
    /// <param name="variables">Переменные для запроса (опционально).</param>
    /// <returns>Результат выполнения запроса.</returns>
    [HttpPost("query")]
    public async Task<IActionResult> ExecuteQuery([FromBody] GraphQLRequest request)
    {
        try
        {
            var result = await _graphQLClient.ExecuteQueryAsync<dynamic>(request.Query, request.Variables);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

/// <summary>
/// Модель для запроса GraphQL.
/// </summary>
public class GraphQLRequest
{
    public string Query { get; set; }
    public object? Variables { get; set; }
}