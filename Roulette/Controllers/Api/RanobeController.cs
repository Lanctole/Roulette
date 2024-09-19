using Microsoft.AspNetCore.Mvc;
using Roulette.Helpers;
using Roulette.Services;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Enums;
using ShikimoriSharp.Settings;

namespace Roulette.Controllers.Api;

/// <summary>
///     Контроллер для работы с light novel (ranobe).
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class RanobeController
    (ShikimoriApiConnectorService apiConnectorService, ShikiDataHelper shikiDataHelper) : ShikimoriController(
        apiConnectorService, shikiDataHelper)
{
    /// <summary>
    ///     Возвращает список light novel (ranobe), соответствующих указанным критериям.
    /// </summary>
    /// <param name="score">Минимальная оценка light novel [0-10].</param>
    /// <param name="season">Время выхода light novel [summer_2017, 2016].</param>
    /// <param name="publisher">Издатель light novel.</param>
    /// <param name="genre">Жанр light novel.</param>
    /// <param name="censored">Прятать ли анцензоред [true, false].</param>
    /// <param name="search">Параметр поиска для фильтрации light novel.</param>
    /// <param name="status">Статус light novel. Может быть 'вышло', 'выходит' или другое значение.</param>
    /// <param name="order">
    ///     Порядок сортировки [id, id_desc, ranked, kind, popularity, name, aired_on, episodes, status,
    ///     random].
    /// </param>
    /// <param name="limit">Количество возвращаемых результатов.</param>
    /// <param name="page">Номер страницы для пагинации.</param>
    /// <returns>
    ///     Список light novel, удовлетворяющих указанным условиям.
    ///     Возвращает код состояния 200 (OK) при успешном выполнении.
    ///     В случае внутренней ошибки сервера возвращает код состояния 500 (Internal Server Error).
    /// </returns>
    [HttpGet("ranobes")]
    public async Task<IActionResult> GetRanobes(
        [FromQuery] int? score = null,
        [FromQuery] string? season = null,
        [FromQuery] int[]? publisher = null,
        [FromQuery] string? genre = null,
        [FromQuery] bool? censored = null,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] Order? order = null,
        [FromQuery] int limit = 5,
        [FromQuery] int? page = 1)
    {
        if (limit <= 0) return BadRequest("Параметр 'limit' должен быть положительным числом.");

        if (page <= 0) return BadRequest("Параметр 'page' должен быть положительным числом.");

        var settings = new RanobeRequestSettings
        {
            order = order,
            score = score,
            season = season,
            publisher = publisher,
            genre = genre,
            censored = censored,
            search = search,
            status = status,
            limit = limit,
            page = page
        };
        try
        {
            var ranobes = await _apiConnectorService.GetRanobes(settings);
            return Ok(ranobes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
        }
    }

    /// <summary>
    ///     Возвращает light novel (ranobe) по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор light novel. Должен быть положительным числом.</param>
    /// <returns>
    ///     Детальная информация о light novel с указанным ID.
    ///     В случае внутренней ошибки сервера возвращает код состояния 500 (Internal Server Error).
    /// </returns>
    [HttpGet("ranobes/{id}")]
    public async Task<IActionResult> GetRanobeById(long id)
    {
        if (id <= 0) return BadRequest("Идентификатор light novel должен быть положительным числом.");

        try
        {
            var ranobe = await _apiConnectorService.GetRanobeById(id);
            return Ok(ranobe);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
        }
    }
}